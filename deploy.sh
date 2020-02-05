#!/bin/sh
cmd='imgup'
file='ImgUp'
src='bin/Release/netcoreapp3/linux-x64/publish/'
dest='/usr/bin/'
os='linux-x64'

dotnet publish -c Release
sudo warp-packer --arch $os --input_dir $src --exec $file --output $dest$cmd
echo $(which $cmd)