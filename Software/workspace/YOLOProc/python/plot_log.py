#!/usr/bin/env python

"""
Parse training log

Evolved from parse_log.sh
"""

import os
import re
import argparse
import csv
from collections import OrderedDict
import matplotlib.pyplot as plt
import matplotlib.cm as cm
import numpy as np


"""
sample output

Loaded: 2.528628 seconds
Region Avg IOU: 0.227440, Class: 0.079891, Obj: 0.553766, No Obj: 0.518204, Avg Recall: 0.055556,  count: 18
Region Avg IOU: 0.106427, Class: 0.056761, Obj: 0.625433, No Obj: 0.514253, Avg Recall: 0.000000,  count: 14
Region Avg IOU: 0.165932, Class: 0.028226, Obj: 0.376734, No Obj: 0.517029, Avg Recall: 0.038462,  count: 26
Region Avg IOU: 0.140938, Class: 0.090396, Obj: 0.422523, No Obj: 0.518810, Avg Recall: 0.034483,  count: 29
Region Avg IOU: 0.193469, Class: 0.061432, Obj: 0.645408, No Obj: 0.514188, Avg Recall: 0.055556,  count: 18
Region Avg IOU: 0.354011, Class: 0.027257, Obj: 0.447333, No Obj: 0.514265, Avg Recall: 0.333333,  count: 9
Region Avg IOU: 0.164411, Class: 0.026563, Obj: 0.517987, No Obj: 0.516759, Avg Recall: 0.000000,  count: 13
Region Avg IOU: 0.115462, Class: 0.021012, Obj: 0.316927, No Obj: 0.515158, Avg Recall: 0.000000,  count: 9
1: 292.968048, 292.968048 avg, 0.000100 rate, 4.789057 seconds, 64 images
"""

def parse_log(path_to_log):
    """Parse log file
    Returns (train_dict_list, test_dict_list)
    train_dict_list and test_dict_list are lists of dicts that define the table
    rows
    """
    rgnstr= 'Region Avg IOU: ([\.\deE+-]+), Class: ([\.\deE+-]+), Obj: ([\.\deE+-]+), No Obj: ([\.\deE+-]+), Avg Recall: ([\.\deE+-]+),  count: (\d+)'
    itrstr='(\d+): ([\.\deE+-]+), ([\.\deE+-]+) avg, ([\.\deE+-]+) rate, ([\.\deE+-]+) seconds, (\d+) images'
    regex_region = re.compile(rgnstr)
    regex_iteration = re.compile(itrstr)

    # Pick out lines of interest
    iteration = 0
    train_dict_list = []
    train_row = None
    IOU=0.0
    Class=0.0
    Obj=0.0
    NoObj=0.0
    Recall=0.0
    rcnt=0

    with open(path_to_log) as f:
        for line in f:
            region_match = regex_region.search(line)
            if region_match:
                IOU += float(region_match.group(1))
                Class += float(region_match.group(2))
                Obj += float(region_match.group(3))
                NoObj += float(region_match.group(4))
                Recall += float(region_match.group(5))
                rcnt += 1                
                region_match = regex_region.search(line)
                
            iteration=0
            loss = 0.0
            aveloss = 0.0
            rate = 0.0
            time=0.0
            images = 0
            
            iteration_match = regex_iteration.search(line)
            if iteration_match:
                if rcnt>0:
                    IOU /= rcnt
                    Class /= rcnt
                    Obj /= rcnt
                    NoObj /= rcnt
                    Recall /= rcnt
                    
                
                iteration = float(iteration_match.group(1))
                loss = float(iteration_match.group(2))
                aveloss = float(iteration_match.group(3))
                rate = float(iteration_match.group(4))
                time = float(iteration_match.group(5))
                images = float(iteration_match.group(6))

                row = OrderedDict([
                    ('iter', iteration),
                    ('loss', loss),
                    ('aveloss', aveloss),
                    ('rate', rate),
                    ('images', images),
                    ('IOU', IOU),
                    ('class', Class),
                    ('obj', Obj),
                    ('noobj', NoObj),
                    ('recall', Recall)
                ])
                IOU=0.0
                Class=0.0
                Obj=0.0
                NoObj=0.0
                Recall=0.0
                rcnt=0
                train_dict_list.append(row)

    return train_dict_list

def plot(itrs,loss,IOU,cls,obj,nobj,recall,plot_all):
    
    fig, ax1 = plt.subplots()
    ax1.plot(itrs,loss,color='k')
    ax1.set_xlabel('Iteration')
    #plt.figure(figsize=(10, 8))
    ax1.set_ylabel('Loss', color='k')
    if plot_all:
        nplots=5
    else:
        nplots=3
    colors = iter(cm.rainbow(np.linspace(0, 1, nplots)))
    ax2 = ax1.twinx()
    ax2.set_ylim([0,1.0]) 
    ax2.plot(itrs, IOU,color=next(colors))
    ax2.plot(itrs, cls,color=next(colors))

    if plot_all:
        ax2.plot(itrs, obj,color=next(colors))
        ax2.plot(itrs, nobj,color=next(colors))
        ax2.plot(itrs, recall,color=next(colors))

    plt.title('Training Plot', fontsize=20)
    ax1.grid(True)
    
    ax1.legend(['loss'], loc='upper left')
    if plot_all:
        ax2.legend(['IOU', 'class' ,'obj', 'nobj', 'recall'], loc='upper right')
    else:
        ax2.legend(['IOU', 'class'], loc='upper right')
      

    plt.show()

def plot_lists(dict_lists,plot_all):
    Loss=[]
    Itrs=[]
    IOU=[]
    Class=[]
    Recall=[]
    Obj=[]
    NoObj=[]

    for index in range(len(dict_lists)):
        line = dict_lists[index]
        # print(line)
        Itrs.append(line['iter'])
        Loss.append(line['aveloss'])
        IOU.append(line['IOU'])
        Class.append(line['class'])
        Recall.append(line['recall'])
        Obj.append(line['obj'])
        NoObj.append(line['noobj'])

    plot(Itrs,Loss,IOU,Class,Obj,NoObj,Recall,plot_all)

def parse_args():
    description = ('Plot/Parse a faster-rcnn training log')
    parser = argparse.ArgumentParser(description=description)

    parser.add_argument('logfile_path',
                        help='Path to log file (required)')

    parser.add_argument('--all', '-all', action='store_true')


    args = parser.parse_args()
    return args

Debug=False
if Debug == True:
    logfile_path='/home/dean/AI/darknet/logs/voc_train.txt'
    dict_list=[]
    dict_list = parse_log(logfile_path)
    plot_lists(dict_list,True)

def main():
    args = parse_args()
    logfile_path=args.logfile_path
    dict_list=[]
    dict_list = parse_log(logfile_path)

    plot_lists(dict_list,args.all)

if __name__ == '__main__':
    main()
