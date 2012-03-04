using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace DigitalRune.Game.UI
{
  /// <summary>
  /// Loads/saves objects from/to a XML file. (Currently, only loading objects is implemented!)
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="Load"/> method reads an XML document, creates and returns the objects specified 
  /// in the XML document. Here is an example XML file:
  /// </para>
  /// <example>
  /// <code lang="csharp">
  /// <![CDATA[
  /// <?xml version="1.0" encoding="utf-8" ?>
  /// <Layout DefaultNamespace="DigitalRune.Game.UI.Controls">
  ///   <MyClass Namespace="NamespaceFoo1.Foo2">
  ///     <Data>123</Data>    
  ///   </MyClass>
  ///   
  ///   <Button Name="Button0">
  ///     <X>100</X>
  ///     <VerticalAlignment>Bottom</VerticalAlignment>
  ///     <Height>50</Height>
  ///     <Width>100</Width>    
  ///     <Text>This is a test button...</Text>
  ///   </Button>
  /// 
  ///   <Window Name="Window1" Namespace="DigitalRune.Game.UI.Controls">
  ///     <Content>
  ///       <StackPanel Name="Panel0">
  ///         <Children>
  ///           <TextBox Name="TextBox0">
  ///             <Text>Default text</Text>
  ///           </TextBox>
  ///           <TextBox Name="TextBox1">
  ///             <Text>Default text</Text>
  ///           </TextBox>
  ///         </Children>
  ///       </StackPanel>
  ///     </Content>
  ///   </Window>
  /// </Layout>
  /// ]]>
  /// </code>
  /// <para>
  /// The XML file can contain any types of classes. The types must be defined in the this assembly 
  /// or in an assembly referenced by the <see cref="Assemblies"/> list. Properties of the objects 
  /// can be specified in XML attributes or in XML elements. Each type can have a <c>Namespace</c> 
  /// attribute. For example, if the XML above is loaded, an instance of 
  /// <c>NamespaceFoo1.Foo2.MyClass</c> is created. If no namespace attribute is specified, the
  /// <c>DefaultNamespace</c> of the root node <c>Layout</c> is used.
  /// </para>
  /// <para>
  /// Properties that are lists (interface <see cref="IList"/>) are also supported, see 
  /// <c>Window.Children</c> in the example above. The collection property (e.g. 
  /// <c>Window.Children</c>) is not initialized by the <see cref="LayoutSerializer"/>, the 
  /// <see cref="LayoutSerializer"/> will only try to add items to the collection.
  /// </para>
  /// </example>
  /// </remarks>
  public class LayoutSerializer
  {
    /// <summary>
    /// Gets assemblies that contain the types.
    /// </summary>
    /// <value>The assemblies that contain the types.</value>
    public List<Assembly> Assemblies
    {
      get { return _assemblies; }
    }
    private readonly List<Assembly> _assemblies = new List<Assembly>();


    /// <summary>
    /// Gets or sets the default namespace specified in the "Layout" node of the XML file.
    /// </summary>
    /// <value>The default namespace.</value>
    /// <remarks>
    /// This property is only set while loading objects. It will automatically be reset to 
    /// null when <see cref="Load"/> is finished.
    /// </remarks>
    protected string DefaultNamespace { get; private set; }


    /// <summary>
    /// Loads the objects specified in the given XML container.
    /// </summary>
    /// <param name="container">The container, usually a <see cref="XDocument"/>.</param>
    /// <returns>The instances created from the XML definition.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="container"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="SerializationException">
    /// Error while loading the objects. See exception message.
    /// </exception>
    public IEnumerable Load(XContainer container)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      // Get root node.
      var layout = container.Element("Layout");
      if (layout == null)
        throw new SerializationException("Could not find XML node 'Layout'.");

      // Get default namespace from attribute.
      DefaultNamespace = (string)layout.Attribute("DefaultNamespace");

      // Load all objects and add to list.
      List<object> objects = new List<object>();
      foreach (var element in layout.Elements())
        objects.Add(CreateInstance(element));

      DefaultNamespace = null;

      return objects;
    }


    private object CreateInstance(XElement element)
    {
      // Get type. Probably need to search other assemblies too.
      var type = OnGetType(element);
      if (type == null)
      {
        string message = string.Format("Could not find type '{0}'.", element.Name);
        throw new SerializationException(message);
      }

      // Create instance. 
      var obj = OnCreateInstance(type, element);
      if (obj == null)
      {
        string message = string.Format("Could not create object '{0}' (Name='{1}').", element.Name, element.Attribute("Name"));
        throw new SerializationException(message);
      }

      // Load the properties of this instance.
      LoadProperties(obj, element);

      return obj;
    }


    /// <summary>
    /// Called to find a type for the given XML element.
    /// </summary>
    /// <param name="element">The XML element.</param>
    /// <returns>The type for the given XML element.</returns>
    /// <remarks>
    /// Per default, this method searches the current assembly and the <see cref="Assemblies"/>
    /// for the type. Derived classes can override this method to retrieve the type from other
    /// assemblies.
    /// </remarks>
    protected virtual Type OnGetType(XElement element)
    {
      // Get type name including namespace. Namespace can be specified as XML attribute. If
      // not specified, we assume that it is one of our UIControls.      
      string namespaceString = (string)element.Attribute("Namespace") ?? DefaultNamespace;
      string typeName = string.IsNullOrEmpty(namespaceString)
                     ? element.Name.ToString()
                     : namespaceString + "." + element.Name;

      // Try current assembly and specified assemblies until we find the type.
      var type = Type.GetType(typeName, false);
      if (type != null)
        return type;

      foreach (var assembly in Assemblies)
      {
        type = Type.GetType(typeName + ", " + assembly.FullName);
        if (type != null)
          return type;
      }

      return null;
    }


    /// <summary>
    /// Called when a game instance needs to be created.
    /// </summary>
    /// <param name="type">The type of the object.</param>
    /// <param name="element">The XML node with data for the instance.</param>
    /// <returns>A new instance of the <paramref name="type"/>.</returns>
    /// <remarks>
    /// Per default, the parameterless constructor of <paramref name="type"/> is invoked. This
    /// method can be overridden in derived classes to support types that do not have a 
    /// parameterless default constructor or that require a special initialization. 
    /// </remarks>
    protected virtual object OnCreateInstance(Type type, XElement element)
    {
      return Activator.CreateInstance(type);
    }


    private void LoadProperties(object instance, XElement controlElement)
    {
      // Properties can be specified in XML attributes and XML nodes.
      // Properties of type IList are also supported, but the list must not be null.

      var type = instance.GetType();

      // ----- XML Attributes
      foreach (var attribute in controlElement.Attributes())
      {
        var name = attribute.Name.ToString();
        var value = attribute.Value;

        try
        {
          PropertyInfo property = type.GetProperty(name);
          if (property != null)
            property.SetValue(instance, ObjectHelper.Parse(property.PropertyType, value), null);
        }
        catch (Exception exception)
        {
          string message = string.Format(
            "Could not parse or set value of property '{0}'. Value='{1}'", 
            name, 
            value);

          throw new SerializationException(message, exception);
        }
      }

      // ----- XML Nodes
      foreach (var element in controlElement.Elements())
      {
        var name = element.Name.ToString();

        // Check if the property value consist of several other XML nodes. In this case 
        // the property must be an IList to which we can add the child nodes.
        var numberOfChildElements = element.Elements().Count();

        try
        {
          // Get a .NET property.
          PropertyInfo property = type.GetProperty(name);
          if (property == null)
            continue;

          // Check if property is an IList.
          IList list = property.GetValue(instance, null) as IList;
          if (list == null && numberOfChildElements > 1)
          {
            string message = string.Format(
              "Could not set property '{0}'. The property should implement IList and must not be null.", 
              name);

            throw new SerializationException(message);
          }

          if (numberOfChildElements == 0)
          {
            // No child XML node. element.Value is a string.
            if (list != null)
              list.Add(element.Value);
            else
              property.SetValue(instance, ObjectHelper.Parse(property.PropertyType, element.Value), null);
          }
          else if (numberOfChildElements == 1)
          {
            // XML node contains another child XML node. 
            var value = CreateInstance(element.Elements().First());

            if (list != null)
              list.Add(value);
            else
              property.SetValue(instance, value, null);
          }
          else
          {
            // There are several child elements.
            Debug.Assert(list != null, "The property must implement IList.");
            foreach (var valueElement in element.Elements())
            {
              var value = CreateInstance(valueElement);
              list.Add(value);
            }
          }
        }
        catch (Exception exception)
        {
          string message = string.Format(
            "Could not parse or set value of property '{0}'. Value='{1}'", 
            name, 
            element.Value);

          throw new SerializationException(message, exception);
        }
      }
    }
  }
}
