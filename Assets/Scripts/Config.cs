using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Config : MonoBehaviour
{
    //�w�q�s��´Ѵѫ����r��
    public Dictionary<string, Dictionary<string, Tuple<string, int>>> valueModelXTest;
    //�w�q�s��մѴѫ����r��
    public Dictionary<string, Dictionary<string, Tuple<string, int>>> valueModelOTest;
    //�w�q�ഫ�᪺�Ѧ�C��
    public List<Dictionary<string, List<Tuple<string, int>>>> valueModelX { get; private set; }
    public List<Dictionary<string, List<Tuple<string, int>>>> valueModelO { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InitializeValueModels();
        TransformValueModels();
    }

    void InitializeValueModels()
    {
        valueModelXTest = new Dictionary<string, Dictionary<string, Tuple<string, int>>>
        {
            { "5", new Dictionary<string, Tuple<string, int>>
                {
                    { "5", Tuple.Create("XXXXX", 1000) }
                }
            },
            { "4", new Dictionary<string, Tuple<string, int>>
                {
                   { "4p_0_0", Tuple.Create(" XXXX ", 400) }, //���|

                    { "4_0_0", Tuple.Create(" XXXX", 100) },
                    { "4_0_1", Tuple.Create(" XXXXO", 100) },
                    { "4_0_2", Tuple.Create("XXXX ", 100) },
                    { "4_0_3", Tuple.Create("OXXXX ", 100) },

                    { "4_1_0", Tuple.Create("  XXX X", 120) },
                    { "4_1_1", Tuple.Create("  XXX XO", 120) },
                    { "4_1_2", Tuple.Create(" XXX X", 101) },
                    { "4_1_3", Tuple.Create(" XXX XO", 101) },
                    { "4_1_4", Tuple.Create("XXX X", 100) },
                    { "4_1_5", Tuple.Create("OXXX X", 100) },
                    { "4_1_6", Tuple.Create("OXXX XO", 100) },
                    { "4_1_7", Tuple.Create("  XXX X ", 50) },
                    { "4_1_8", Tuple.Create(" XXX X ", 50) },
                    { "4_1_9", Tuple.Create("OXXX X ", 50) },
                    { "4_1_10", Tuple.Create(" XXX X ", 50) },
                    { "4_1_11", Tuple.Create("XXX X ", 50) },
                    { "4_1_12", Tuple.Create("XXX XO", 50) },

                    { "4_2_0", Tuple.Create("   XX XX   ", 110) },
                    { "4_2_1", Tuple.Create("   XX XX  ", 108) },
                    { "4_2_2", Tuple.Create("   XX XX ", 106) },
                    { "4_2_3", Tuple.Create("   XX XX", 105) },
                    { "4_2_4", Tuple.Create("   XX XXO", 105) },
                    { "4_2_5", Tuple.Create("  XX XX   ", 108) },
                    { "4_2_6", Tuple.Create("  XX XX  ", 106) },
                    { "4_2_7", Tuple.Create("  XX XX ", 104) },
                    { "4_2_8", Tuple.Create("  XX XX", 102) },
                    { "4_2_9", Tuple.Create("  XX XXO", 102) },
                    { "4_2_10", Tuple.Create(" XX XX   ", 106) },
                    { "4_2_11", Tuple.Create(" XX XX  ", 103) },
                    { "4_2_12", Tuple.Create(" XX XX ", 102) },
                    { "4_2_13", Tuple.Create(" XX XX", 101) },
                    { "4_2_14", Tuple.Create(" XX XXO", 101) },
                    { "4_2_15", Tuple.Create("XX XX   ", 105) },
                    { "4_2_16", Tuple.Create("XX XX  ", 102) },
                    { "4_2_17", Tuple.Create("XX XX ", 101) },
                    { "4_2_18", Tuple.Create("XX XXO", 100) },
                    { "4_2_19", Tuple.Create("OXX XX   ", 105) },
                    { "4_2_20", Tuple.Create("OXX XX  ", 102) },
                    { "4_2_21", Tuple.Create("OXX XX ", 101) },
                    { "4_2_22", Tuple.Create("OXX XX", 100) },
                    { "4_2_23", Tuple.Create("OXX XXO", 100) },
                    { "4_2_24", Tuple.Create("XX XX", 100) },

                    { "4_3_0", Tuple.Create(" X XXX  ", 121) },
                    { "4_3_1", Tuple.Create(" X XXX ", 102) },
                    { "4_3_2", Tuple.Create(" X XXX", 101) },
                    { "4_3_3", Tuple.Create(" X XXXO", 101) },
                    { "4_3_4", Tuple.Create("X XXX  ", 120) },
                    { "4_3_5", Tuple.Create("X XXX ", 101) },
                    { "4_3_6", Tuple.Create("OX XXX", 100) },
                    { "4_3_7", Tuple.Create("X XXXO", 100) },
                    { "4_3_8", Tuple.Create("OX XXX  ", 120) },
                    { "4_3_9", Tuple.Create("OX XXX ", 101) },
                    { "4_3_10", Tuple.Create("OX XXXO", 100) },
                    { "4_3_11", Tuple.Create("X XXX", 100) },
                }
            },
            { "3", new Dictionary<string, Tuple<string, int>>
               {
                    { "3p_0_0", Tuple.Create("  XXX  ", 60) }, //���T
                    { "3p_0_1", Tuple.Create("  XXX ", 30) }, //���T
                    { "3p_0_2", Tuple.Create(" XXX  ", 30) }, //���T
                    { "3_0_0", Tuple.Create("  XXX", 25) },
                    { "3_0_1", Tuple.Create("  XXXO", 25) },
                    { "3_0_2", Tuple.Create(" XXX ", 25) },
                    { "3_0_3", Tuple.Create("XXX  ", 25) },
                    { "3_0_4", Tuple.Create("OXXX  ", 25) },

                    { "3p_1_0", Tuple.Create("   XX X ", 37) }, //���T
                    { "3p_1_1", Tuple.Create("  XX X ", 31) }, //���T
                    { "3p_1_2", Tuple.Create("  XX X ", 31) }, //���T
                    { "3p_1_3", Tuple.Create(" XX X ", 30) }, //���T

                    { "3_1_0", Tuple.Create("   XX X", 27) },
                    { "3_1_1", Tuple.Create("   XX XO", 27) },
                    { "3_1_2", Tuple.Create("  XX X", 26) },
                    { "3_1_3", Tuple.Create("  XX XO", 26) },
                    { "3_1_4", Tuple.Create(" XX X", 25) },
                    { "3_1_5", Tuple.Create(" XX XO", 25) },
                    { "3_1_6", Tuple.Create("XX X ", 25) },
                    { "3_1_7", Tuple.Create("OXX X ", 25) },

                    { "3p_2_0", Tuple.Create(" X XX   ", 37) }, //���T
                    { "3p_2_1", Tuple.Create(" X XX  ", 31) }, //���T
                    { "3p_2_2", Tuple.Create(" X XX ", 30) }, //���T
                    { "3p_2_3", Tuple.Create(" X XX", 25) },
                    { "3_2_0", Tuple.Create(" X XXO", 25) },
                    { "3_2_1", Tuple.Create("X XX   ", 27) },
                    { "3_2_2", Tuple.Create("X XX  ", 26) },
                    { "3_2_3", Tuple.Create("X XX ", 25) },
                    { "3_2_4", Tuple.Create("OX XX   ", 27) },
                    { "3_2_5", Tuple.Create("OX XX  ", 26) },
                    { "3_2_6", Tuple.Create("OX XX ", 25) },

                    { "3_3_0", Tuple.Create("  X X X  ", 27) },
                    { "3_3_1", Tuple.Create("  X X X ", 26) },
                    { "3_3_2", Tuple.Create("  X X X", 25) },
                    { "3_3_3", Tuple.Create("  X X XO", 25) },
                    { "3_3_4", Tuple.Create(" X X X  ", 26) },
                    { "3_3_5", Tuple.Create(" X X X ", 25)},
                    { "3_3_6", Tuple.Create(" X X X", 24) },
                    { "3_3_7", Tuple.Create(" X X XO", 24) },
                    { "3_3_8", Tuple.Create("X X X  ", 25) },
                    { "3_3_9", Tuple.Create("X X X ", 24) },
                    { "3_3_10", Tuple.Create("X X XO", 24) },
                    { "3_3_11", Tuple.Create("OX X X  ", 25) },
                    { "3_3_12", Tuple.Create("OX X X ", 24) },
                    { "3_3_13", Tuple.Create("OX X X", 23) },
                    { "3_3_14", Tuple.Create("OX X XO", 23) },
                    { "3_3_15", Tuple.Create("X X X", 23) },
                }
            },  
            { "2", new Dictionary<string, Tuple<string, int>>
                {
                //2_0���ѫ��ܤ֭n�]�p�@��
                    { "2_0_0", Tuple.Create("   XX   ", 8) },
                    { "2_0_1",Tuple.Create("   XX  ", 4) },
                    { "2_0_2", Tuple.Create("   XX ", 3) },
                    { "2_0_3", Tuple.Create("   XX", 1) },
                    { "2_0_4", Tuple.Create("   XXO", 1) },
                    { "2_0_5", Tuple.Create("  XX   ", 4) },
                    { "2_0_6", Tuple.Create("  XX  ", 3) },
                    { "2_0_7", Tuple.Create("  XX ", 2) },
                    { "2_0_8", Tuple.Create(" XX   ", 3) },
                    { "2_0_9", Tuple.Create(" XX  ", 2) },
                    { "2_0_10", Tuple.Create("XX   ", 1) }, 
                    { "2_0_11", Tuple.Create("OXX   ", 1) },

                    { "2_1_0", Tuple.Create("  X X  ", 4) },
                    { "2_1_1", Tuple.Create("  X X ", 3) },
                    { "2_1_2", Tuple.Create("  X X", 1) },
                    { "2_1_3", Tuple.Create("  X XO", 1) },
                    { "2_1_4", Tuple.Create(" X X  ", 3) },
                    { "2_1_5", Tuple.Create(" X X ", 2) },
                    { "2_1_6", Tuple.Create("X X  ", 1) },
                    { "2_1_7", Tuple.Create("OX X  ", 1) },

                    { "2_2_0", Tuple.Create("  X  X  ", 4) },
                    { "2_2_1", Tuple.Create("  X  X ", 3) },
                    { "2_2_2", Tuple.Create("  X  X", 2) },
                    { "2_2_3", Tuple.Create("  X  XO", 2) },
                    { "2_2_4", Tuple.Create(" X  X  ", 3) },
                    { "2_2_5", Tuple.Create(" X  X ", 2) },
                    { "2_2_6", Tuple.Create(" X  X", 1) },
                    { "2_2_7", Tuple.Create(" X  XO", 1) },
                    { "2_2_8", Tuple.Create("X  X  ", 2) },
                    { "2_2_9", Tuple.Create("X  X ", 1) },
                    { "2_2_10", Tuple.Create("OX  X  ", 1) },
                    { "2_2_11", Tuple.Create("OX  X ", 1) },
                    { "2_2_12", Tuple.Create("   X  X   ", 1) },
                    { "2_2_13", Tuple.Create("  X  X   ", 1) },
                    { "2_2_14", Tuple.Create(" X  X   ", 1) },
                    { "2_2_15", Tuple.Create("X  X   ", 1) },
                    { "2_2_16", Tuple.Create("   X  X  ", 1) },
                    { "2_2_17", Tuple.Create("   X  X ", 1) },
                    { "2_2_18", Tuple.Create("   X  X", 1) },
                    { "2_2_19", Tuple.Create("OX  X   ", 1) },
                    { "2_2_20", Tuple.Create("   X  XO", 1) },

                    { "2_3_0",  Tuple.Create(" X   X ", 3) },
                    { "2_3_1", Tuple.Create(" X   X", 2) },
                    { "2_3_2", Tuple.Create(" X   XO", 2) },
                    { "2_3_3", Tuple.Create("X   X ", 2) },
                    { "2_3_4", Tuple.Create("X   X", 1) },
                    { "2_3_5", Tuple.Create("X   XO", 1) },
                    { "2_3_6", Tuple.Create("OX   X ", 2) },
                    { "2_3_7", Tuple.Create("OX   X", 1) },
                    { "2_3_8", Tuple.Create("OX   XO", 1) },
                    { "2_3_9", Tuple.Create("   X   X   ", 1) },
                    { "2_3_10", Tuple.Create("   X   X  ", 1) },
                    { "2_3_11", Tuple.Create("   X   X", 1) },
                    { "2_3_12", Tuple.Create("  X   X   ", 1) },
                    { "2_3_13", Tuple.Create("  X   X  ", 1) },
                    { "2_3_14", Tuple.Create("  X   X ", 1) },
                    { "2_3_15", Tuple.Create("  X   X", 1) },
                    { "2_3_16", Tuple.Create(" X   X   ", 1) },
                    { "2_3_17", Tuple.Create(" X   X  ", 1) },
                    { "2_3_18", Tuple.Create("X   X   ", 1) },
                    { "2_3_19", Tuple.Create("X   X  ", 1) },
                    { "2_3_20", Tuple.Create("OX   X   ", 1) },
                    { "2_3_21", Tuple.Create("OX   X  ", 1) },
                    { "2_3_22", Tuple.Create("   X   XO", 1) },
                    { "2_3_23", Tuple.Create("  X   XO", 1) }
                }
            }
        };

        valueModelOTest = new Dictionary<string, Dictionary<string, Tuple<string, int>>>();
        foreach(var outer in valueModelXTest) //�M���Ĥ@�h�r��
        {
            var outerKey = outer.Key; //�o��Ĥ@�h�r�媺Key
            var innerDic = outer.Value; //�o��Ĥ@�h�r�媺�ȡA�]�N�O�ĤG�h�r��

            foreach(var inner in innerDic) //�M���ĤG�h�r��
            {
                var innerKey = inner.Key; //�o��ĤG�h�r�媺Key�A�]�N�O�Ѧ�s��
                var tupleValue = inner.Value; //�o��ĤG�h�r�媺�ȡA�]�N�O�@��<�ѫ��r��, int����>��Tuple
                //��Tuple���Ĥ@�Ӥ�������'X'�ഫ��'O'�A'O'�ഫ��'X'
                var newString = new string(tupleValue.Item1.Select(c => c == 'X' ? 'O' : c == 'O' ? 'X' : c).ToArray());
                var newTuple = Tuple.Create(newString, tupleValue.Item2); //�o��@�Ӵ����᪺�sTuple

                if (!valueModelOTest.ContainsKey(outerKey)) //�ˬdvalueModelOTest�O�_�]�t��e���Ĥ@�h�r�媺Key
                {
                    //�p�G���]�t�A�h�Ыؤ@�ӷs�����h(�ĤG�h)�r��A�òK�[��valueModelOTest��
                    valueModelOTest[outerKey] = new Dictionary<string, Tuple<string, int>>(); 
                }
                //���Ĥ@�h�r�媺Value(�]�N�O�ĤG�h�r��)�A�]���n�A���ĤG�h�r�媺value�~�|������Tuple<�ѫ��r��, int����>
                valueModelOTest[outerKey][innerKey] = newTuple; 
            }
        }
    }
   // �N�r���ഫ��List���A�����
    private List<Dictionary<string, List<Tuple<string, int>>>> TransformModel(
        Dictionary<string, Dictionary<string, Tuple<string, int>>> model)
    {
        var transformedList = new List<Dictionary<string, List<Tuple<string, int>>>>(); //��l���ഫ�᪺�C��
        foreach(var outer in model) //�M���~�h�r��
        {
            var innerDic = outer.Value; //����~�h�r�媺Value�A�]�N�O���h�r��
            var transformedDic = new Dictionary<string, List<Tuple<string, int>>>(); //��l���ഫ�᪺�r��
            foreach(var inner in innerDic) //�M�����h�r��
            {
                //�@�ӷs���C��A�]�t�@�Ӥ����O���h�r�媺value�A�]�N�O�@��Tuple<string, int>
                var tupleList = new List<Tuple<string, int>> { inner.Value };
                //�NtupleList�K�[��transformedDic�r�媺Value�A�]�N�OTuple<string, int>
                transformedDic[inner.Key] = tupleList;
            }
            //�ഫ�᪺�r��K�[��transformedList�A�]�N�O���A Dictionary<string, List<Tuple<string, int>>>
            transformedList.Add(transformedDic);
        }
        return transformedList;
    }

    void TransformValueModels()
    {
        valueModelO = TransformModel(valueModelOTest);
        valueModelX = TransformModel(valueModelXTest);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
