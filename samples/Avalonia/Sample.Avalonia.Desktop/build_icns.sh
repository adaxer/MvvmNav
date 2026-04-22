#!/bin/bash

set -e

SRC="icon-1024.png"
ICONSET="AppIcon.iconset"
OUT="AppIcon.icns"

echo "Creating iconset..."

rm -rf $ICONSET
mkdir $ICONSET

magick $SRC -resize 16x16   $ICONSET/icon_16x16.png
magick $SRC -resize 32x32   $ICONSET/icon_16x16@2x.png
magick $SRC -resize 32x32   $ICONSET/icon_32x32.png
magick $SRC -resize 64x64   $ICONSET/icon_32x32@2x.png
magick $SRC -resize 128x128 $ICONSET/icon_128x128.png
magick $SRC -resize 256x256 $ICONSET/icon_128x128@2x.png
magick $SRC -resize 256x256 $ICONSET/icon_256x256.png
magick $SRC -resize 512x512 $ICONSET/icon_256x256@2x.png
magick $SRC -resize 512x512 $ICONSET/icon_512x512.png
cp $SRC $ICONSET/icon_512x512@2x.png

echo "Converting to icns..."

iconutil -c icns $ICONSET
qlmanage -p AppIcon.icns

echo "Done: $OUT"