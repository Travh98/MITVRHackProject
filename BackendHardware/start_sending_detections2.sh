#!/bin/bash

gst-launch-1.0 -e \
qtiqmmfsrc name=camsrc ! \
video/x-raw(memory:GBM),\
format=NV12,width=1280,height=720,\
framerate=30/1,compression=ubwc ! \
queue ! tee name=split split. ! queue ! qtimetamux name=metamux ! queue ! qtioverlay ! queue ! \
qtimlmetaparser module=json ! qtiredissink sync=false async=false channel="Detection::yolonas::0" \
host="128.31.35.29" port=6379 split. ! queue ! qtimlvconverter ! queue ! qtimlsnpe delegate=dsp \
model=/opt/yolonas.dlc layers="</heads/Mul, /heads/Sigmoid>" ! \
queue ! qtimlvdetection threshold=51.0 results=10 module=yolo-nas labels=/opt/yolonas.labels ! \
text/x-raw ! queue ! metamux