﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Octodecahedron
{
    public static readonly int baseFacesCount = 80;

    public static readonly Vector3[] vertices = new Vector3[] {
            new Vector3(-0.4472136f, 0.8506508f, -0.2763932f),
            new Vector3(-0.5257311f, 0.8090169f, 0.2628656f),
            new Vector3(0f, 1f, 0f),                                //
            new Vector3(-0.4472136f, 0.5257311f, 0.7236068f),
            new Vector3(0f, 0.8090169f, 0.5877853f),
            new Vector3(0.4472136f, 0.8506508f, 0.2763932f),
            new Vector3(0.5257311f, 0.8090169f, -0.2628656f),
            new Vector3(0.4472136f, 0.5257311f, -0.7236068f),
            new Vector3(0f, 0.8090169f, -0.5877853f),
            new Vector3(0.8506508f, 0.5f, 0.1624598f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0.8506508f, 0.309017f, -0.4253253f),
            new Vector3(0.525731f, 0.5f, 0.6881909f),
            new Vector3(0.4472136f, 0f, 0.8944272f),
            new Vector3(0.8506508f, 0f, 0.5257311f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(-0.8506508f, 0.5f, -0.1624598f),
            new Vector3(-0.8506508f, 0f, -0.5257311f),
            new Vector3(-0.525731f, 0.5f, -0.6881909f),
            new Vector3(-0.4472136f, 0f, -0.8944272f),
            new Vector3(-0.8506508f, 0.309017f, 0.4253253f),
            new Vector3(-0.8506508f, -0.309017f, 0.4253253f),
            new Vector3(-0.4472136f, -0.5257311f, 0.7236068f),
            new Vector3(-0.5257311f, 0f, 0.8506508f),
            new Vector3(-0f, 0.309017f, -0.9510564f),
            new Vector3(0f, 0.309017f, 0.9510564f),
            new Vector3(0.5257311f, 0f, -0.8506508f),
            new Vector3(0.4472136f, -0.5257311f, -0.7236068f),
            new Vector3(-0f, -0.309017f, -0.9510564f),
            new Vector3(0f, -0.309017f, 0.9510564f),
            new Vector3(0.8506508f, -0.309017f, -0.4253253f),
            new Vector3(0.8506508f, -0.5f, 0.1624598f),
            new Vector3(0.4472136f, -0.8506508f, 0.2763932f),
            new Vector3(0.5257311f, -0.8090169f, -0.2628656f),
            new Vector3(0.525731f, -0.5f, 0.6881909f),
            new Vector3(0f, -0.8090169f, 0.5877853f),
            new Vector3(-0.8506508f, -0.5f, -0.1624598f),
            new Vector3(-0.525731f, -0.5f, -0.6881909f),
            new Vector3(-0.4472136f, -0.8506508f, -0.2763932f),
            new Vector3(-0.5257311f, -0.8090169f, 0.2628656f),
            new Vector3(0f, -1f, 0f),                                 //
            new Vector3(0f, -0.8090169f, -0.5877853f)
        };

    public static readonly int[] triangles = new int[] {
        0,1,2,
        3,4,1,
        5,2,4,
        1,4,2,
        5,6,2,
        7,8,6,
        0,2,8,
        6,8,2,
        5,9,6,
        10,11,9,
        7,6,11,
        9,11,6,
        5,12,9,
        13,14,12,
        10,9,14,
        12,14,9,
        15,16,17,
        0,18,16,
        19,17,18,
        16,18,17,
        0,16,1,
        15,20,16,
        3,1,20,
        16,20,1,
        15,21,20,
        22,23,21,
        3,20,23,
        21,23,20,
        0,8,18,
        7,24,8,
        19,18,24,
        8,24,18,
        5,4,12,
        3,25,4,
        13,12,25,
        4,25,12,
        7,26,24,
        27,28,26,
        19,24,28,
        26,28,24,
        3,23,25,
        22,29,23,
        13,25,29,
        23,29,25,
        10,30,11,
        27,26,30,
        7,11,26,
        30,26,11,
        10,31,30,
        32,33,31,
        27,30,33,
        31,33,30,
        13,34,14,
        32,31,34,
        10,14,31,
        34,31,14,
        32,34,35,
        13,29,34,
        22,35,29,
        34,29,35,
        15,17,36,
        19,37,17,
        38,36,37,
        17,37,36,
        22,21,39,
        15,36,21,
        38,39,36,
        21,36,39,
        22,39,35,
        38,40,39,
        32,35,40,
        39,40,35,
        38,37,41,
        19,28,37,
        27,41,28,
        37,28,41,
        32,40,33,
        38,41,40,
        27,33,41,
        40,41,33
    };

    // Each index lists a triangle face.
    public static readonly int[] neighbors = new int[] {
        3,6,20,
        3,22,33,
        3,4,32,
        0,1,2,
        2,7,8,
        7,10,29,
        0,7,28,
        4,5,6,
        4,11,12,
        11,14,44,
        5,11,46,
        8,9,10,
        8,15,32,
        15,34,52,
        9,15,54,
        12,13,14,
        19,21,60,
        19,20,28,
        19,30,61,
        16,17,18,
        0,17,23,
        16,23,24,
        1,23,26,
        20,21,22,
        21,27,65,
        27,41,64,
        22,27,40,
        24,25,26,
        6,17,31,
        5,31,36,
        18,31,38,
        28,29,30,
        2,12,35,
        1,35,40,
        13,35,42,
        32,33,34,
        29,39,46,
        39,45,74,
        30,39,73,
        36,37,38,
        26,33,43,
        25,43,58,
        34,43,57,
        40,41,42,
        9,47,48,
        37,47,50,
        10,36,47,
        44,45,46,
        44,51,54,
        51,53,76,
        45,51,78,
        48,49,50,
        13,55,57,
        49,55,56,
        14,48,55,
        52,53,54,
        53,59,70,
        42,52,59,
        41,59,68,
        56,57,58,
        16,63,65,
        18,63,73,
        63,66,72,
        60,61,62,
        25,67,68,
        24,60,67,
        62,67,69,
        64,65,66,
        58,64,71,
        66,71,77,
        56,71,76,
        68,69,70,
        62,75,77,
        38,61,75,
        37,75,78,
        72,73,74,
        49,70,79,
        69,72,79,
        50,74,79,
        76,77,78,
    };
}

