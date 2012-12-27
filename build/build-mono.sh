!/bin/bash

SCRIPT="$(cd "${0%/*}" 2>/dev/null; echo "$PWD"/"${0##*/}")"
SCRIPTPATH=`dirname $SCRIPT`
echo $SCRIPTPATH

sed -e 's/dep\\sqlite\\sqlite-mixed/dep\\sqlite\\sqlite-managed/g' $SCRIPTPATH/../src/Voxeliq/Voxeliq-VS2010.csproj > $SCRIPTPATH/../src/Voxeliq/Voxeliq-Mono.csproj

xbuild $SCRIPTPATH/Voxeliq.Windows.Monogame.sln
