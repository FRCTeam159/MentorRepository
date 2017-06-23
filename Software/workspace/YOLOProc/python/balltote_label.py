import xml.etree.ElementTree as ET
import pickle
import os
from os import listdir, getcwd
from os.path import join

sets=[('train'), ('val'), ('test')]

classes = ["tote", "ball"]

devkit='/home/dean/data/frc-robotics/ball_tote_devkit/data'

def convert(size, box):
    dw = 1./(size[0])
    dh = 1./(size[1])
    x = (box[0] + box[1])/2.0 - 1
    y = (box[2] + box[3])/2.0 - 1
    w = box[1] - box[0]
    h = box[3] - box[2]
    x = x*dw
    w = w*dw
    y = y*dh
    h = h*dh
    return (x,y,w,h)

def convert_annotation(image_id):
    in_file = open('%s/Annotations/%s.xml'%(devkit,image_id))
    out_file = open('%s/labels/%s.txt'%(devkit,image_id), 'w')
    tree=ET.parse(in_file)
    root = tree.getroot()
    size = root.find('size')
    w = int(size.find('width').text)
    h = int(size.find('height').text)

    for obj in root.iter('object'):
        difficult = obj.find('difficult').text
        cls = obj.find('name').text
        if cls not in classes or int(difficult)==1:
            continue
        cls_id = classes.index(cls)
        xmlbox = obj.find('bndbox')
        b = (float(xmlbox.find('xmin').text), float(xmlbox.find('xmax').text), float(xmlbox.find('ymin').text), float(xmlbox.find('ymax').text))
        bb = convert((w,h), b)
        out_file.write(str(cls_id) + " " + " ".join([str(a) for a in bb]) + '\n')

wd = getcwd()

if not os.path.exists('%s/labels'%(devkit)):
    os.makedirs('%s/labels'%(devkit))
image_ids = open('%s/ImageSets/train.txt'%(devkit)).read().strip().split()
list_file = open('balltote_train.txt', 'w')
for image_id in image_ids:
    list_file.write('%s/Images/%s.jpg\n'%(devkit, image_id))
    convert_annotation(image_id)
list_file.close()

