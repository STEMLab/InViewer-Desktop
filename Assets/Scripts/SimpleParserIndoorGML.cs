using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class SimpleParserIndoorGML
{
    enum TAG_TYPE { SELF, OPEN, CLOSE };

    public bool DebugMode = false;

    public List<PosBasedEntity> PosBasedEntities { get; }

    private void log(int level, string value, TAG_TYPE tagType)
    {
        if (DebugMode == false) return;

        if (tagType == TAG_TYPE.OPEN)
        {
            //Console.WriteLine(printTab(level) + "-> " + value);
        }
        else if (tagType == TAG_TYPE.CLOSE)
        {
            //Console.WriteLine(printTab(level) + "<- " + value);
        }
        else
        {
            //Console.WriteLine(printTab(level) + "<> " + value);
        }
    }

    private static string printTab(int numOfRepeat)
    {
        return null;
        //return Repeat(".", numOfRepeat * 1);
    }

    private static string Repeat(string value, int count)
    {
        return null;
        //return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
    }

    //private 

    private string _fileUrl;

    public SimpleParserIndoorGML(string fileUrl)
    {
        _fileUrl = fileUrl;
        PosBasedEntities = new List<PosBasedEntity>();
    }

    public void Load()
    {
        int level = 1;
        Stack<string> tmpStack = new Stack<string>();
        string currentID = "";
        DATA_TYPE currentType = DATA_TYPE.Undefined;

        // Depends on direction. Queue or Stack.
        PosBasedEntity tmpPosSet = new PosBasedEntity("", "", DATA_TYPE.Undefined);

        using (XmlReader reader = XmlReader.Create(_fileUrl))
        {
            while (reader.Read())
            {
                // Self Closing Tag (boundedBy..)
                if (reader.IsStartElement() && reader.IsEmptyElement)
                {
                    log(level, reader.Name, TAG_TYPE.SELF);
                }
                // 시작 태그
                else if (reader.IsStartElement())
                {
                    level++;

                    log(level, reader.Name, TAG_TYPE.OPEN);

                    tmpStack.Push(reader.Name);

                    // DataType 판단.
                    // ---------------------------------------------------------------------------------------------------------------
                    if (reader.LocalName.Equals("cellSpaceMember"))
                    {
                        currentType = DATA_TYPE.CELLSPACE;
                    }
                    else if (reader.LocalName.Equals("GeneralSpace"))
                    {
                        currentType = DATA_TYPE.GENERALSPACE;
                    }
                    else if (reader.LocalName.Equals("TransitionSpace"))
                    {
                        currentType = DATA_TYPE.TRANSITIONSPACE;
                    }
                    else if (reader.LocalName.Equals("cellSpaceBoundaryMember"))
                    {
                        currentType = DATA_TYPE.CELLSPACEBOUNDARY;
                    }
                    else if (reader.LocalName.Equals("transitionMember"))
                    {
                        currentType = DATA_TYPE.TRANSITION;
                    }
                    else if (reader.LocalName.Equals("stateMember"))
                    {
                        currentType = DATA_TYPE.STATE;
                    }
                    // ---------------------------------------------------------------------------------------------------------------

                    // DataType에 따른 ID 선정위치 여부
                    if ((currentType == DATA_TYPE.CELLSPACE || currentType == DATA_TYPE.TRANSITIONSPACE || currentType == DATA_TYPE.GENERALSPACE)
                        && reader.LocalName.Equals("Solid"))
                    {
                        currentID = reader.GetAttribute("gml:id");
                    }

                    if (currentType == DATA_TYPE.CELLSPACEBOUNDARY
                        && reader.LocalName.Equals("Polygon"))
                    {
                        currentID = reader.GetAttribute("gml:id");
                    }

                    if (currentType == DATA_TYPE.TRANSITION
                        && reader.LocalName.Equals("LineString"))
                    {
                        currentID = reader.GetAttribute("gml:id");
                    }

                    if (currentType == DATA_TYPE.STATE
                        && reader.LocalName.Equals("Point"))
                    {
                        currentID = reader.GetAttribute("gml:id");
                    }

                    // 가시화 핵심 태그
                    // gml:LinearRing
                    // gml:Point
                    // gml:LineString
                    if (reader.LocalName == "LinearRing" ||
                        reader.LocalName == "Point" ||
                        reader.LocalName == "LineString")
                    {
                        tmpPosSet = new PosBasedEntity(currentID, reader.LocalName, currentType);
                    }

                    if (reader.LocalName == "pos")
                    {
                        reader.Read();
                        //Console.WriteLine(reader.Value);

                        string[] values = reader.Value.Trim().Split(' ');
                        Vector3 tmpObj = new Vector3();

                        // Unity3D Vector Style.
                        float.TryParse(values[0], out tmpObj.x);
                        float.TryParse(values[1], out tmpObj.z);
                        float.TryParse(values[2], out tmpObj.y);

                        tmpPosSet.vertices.Add(tmpObj);
                    }
                }
                // 닫히는 태그
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    string lastName = tmpStack.Pop();

                    log(level, reader.Name, TAG_TYPE.CLOSE);
                    level--;
                    if (level < 0) level = 0;
                    if (reader.LocalName == "LinearRing")
                    {
                        // 현재 그리기 모듈에서는 첫점과 끝점이 같을 필요가 없다.
                        // 즉, 중복되는 점을 하나 버림.
                        tmpPosSet.vertices.RemoveAt(tmpPosSet.vertices.Count - 1);
                    }

                        if (reader.LocalName == "LinearRing" ||
                        reader.LocalName == "Point" ||
                        reader.LocalName == "LineString")
                    {
                        PosBasedEntities.Add(tmpPosSet);
                    }
                }
                else
                {
                    //Console.WriteLine("** " + printTab(level) + reader.Value.Trim());
                }

            }
        }
    }
}