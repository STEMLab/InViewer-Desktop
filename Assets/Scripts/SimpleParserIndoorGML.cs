using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

class SimpleParserIndoorGML
{
    enum TAG_TYPE { SELF, OPEN, CLOSE };

    public bool DebugMode = false;

    public List<PosBasedEntity> PosBasedEntities { get; }

    public Dictionary<string, Vector3> mapCenter;

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

    public string GetDirectoryName()
    {
        return Path.GetDirectoryName(_fileUrl);
    }

    public SimpleParserIndoorGML(string fileUrl)
    {
        _fileUrl = fileUrl;
        PosBasedEntities = new List<PosBasedEntity>();
    }

    private void FillCenter()
    {

    }

    public void Load()
    {
        int level = 1;
        Stack<string> tmpStack = new Stack<string>();
        string currentID = "";
        DATA_TYPE currentType = DATA_TYPE.Undefined;

        // Depends on direction. Queue or Stack.
        PosBasedEntity tmpPosSet = new PosBasedEntity("", "", DATA_TYPE.Undefined);

        mapCenter = new Dictionary<string, Vector3>();
        Bounds localBounds = new Bounds();

        int idxLocal = 0;
        bool isInterior = true;

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
                        localBounds = new Bounds();
                    }
                    else if (reader.LocalName.Equals("GeneralSpace"))
                    {
                        currentType = DATA_TYPE.GENERALSPACE;
                        localBounds = new Bounds();
                    }
                    else if (reader.LocalName.Equals("TransitionSpace"))
                    {
                        currentType = DATA_TYPE.TRANSITIONSPACE;
                        localBounds = new Bounds();
                    }
                    else if (reader.LocalName.Equals("cellSpaceBoundaryMember"))
                    {
                        currentType = DATA_TYPE.CELLSPACEBOUNDARY;
                        localBounds = new Bounds();
                    }
                    else if (reader.LocalName.Equals("transitionMember"))
                    {
                        currentType = DATA_TYPE.TRANSITION;
                        localBounds = new Bounds();
                    }
                    else if (reader.LocalName.Equals("stateMember"))
                    {
                        currentType = DATA_TYPE.STATE;
                        localBounds = new Bounds();
                    }
                    // ---------------------------------------------------------------------------------------------------------------
                    else if (reader.LocalName.Equals("TextureImage"))
                    {
                        reader.Read();
                        tmpPosSet.texture = reader.Value;
                    }
                    // ---------------------------------------------------------------------------------------------------------------

                    if (reader.LocalName.Equals("interior") && currentType == DATA_TYPE.CELLSPACEBOUNDARY)
                    {
                        isInterior = true;
                        tmpPosSet.interiors.Add(new List<Vector3>());

                        IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
                        int lineNumber = xmlInfo.LineNumber;
                        Debug.Log("Interior Pos: " + lineNumber);

                    }
                    else if (reader.LocalName.Equals("exterior") == true)
                    {
                        isInterior = false;
                    }

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


                    //인테리어와 익스테리어를 구분한다.

                    // 가시화 핵심 태그
                    // gml:LinearRing
                    // gml:Point
                    // gml:LineString
                    if (reader.LocalName == "LinearRing" ||
                        reader.LocalName == "Point" ||
                        reader.LocalName == "LineString")
                    {
                        if (isInterior == false)
                        {
                            tmpPosSet = new PosBasedEntity(currentID, reader.LocalName, currentType);
                        }
                    }

                    if (reader.LocalName == "pos")
                    {
                        reader.Read();
                        //Console.WriteLine(reader.Value);

                        string[] values = reader.Value.Trim().Split(' ');

                        if (values.Length == 3)
                        {
                            Vector3 tmpObj = new Vector3();

                            // Unity3D Vector Style.
                            float.TryParse(values[0], out tmpObj.x);
                            float.TryParse(values[1], out tmpObj.z);
                            float.TryParse(values[2], out tmpObj.y);

                            if (isInterior == true && currentType == DATA_TYPE.CELLSPACEBOUNDARY)
                            {
                                tmpPosSet.interiors.Last().Add(tmpObj);
                            }
                            else
                            {
                                tmpPosSet.exterior.Add(tmpObj);
                            }

                            if (localBounds.min.Equals(new Vector3(0, 0, 0)))
                            {
                                localBounds.SetMinMax(tmpObj, new Vector3(0, 0, 0));
                            }
                            else
                            {
                                localBounds.Encapsulate(tmpObj);
                            }
                        }
                        else if (values.Length == 2)
                        {
                            Vector2 tmpObj = new Vector2();

                            // TextureCoordinate.
                            //float.TryParse(values[0], out tmpObj.y);
                            //float.TryParse(values[1], out tmpObj.x);
                            //tmpObj.x = 1 - tmpObj.x;

                            float.TryParse(values[0], out tmpObj.y);
                            float.TryParse(values[1], out tmpObj.x);

                            tmpObj.x = 1 - tmpObj.x;

                            tmpPosSet.texture_coordinates.Add(tmpObj);
                        }
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
                        //if (tmpPosSet.vertices.Count() > 2)
                        //{
                        //    tmpPosSet.vertices.RemoveAt(tmpPosSet.vertices.Count - 1);
                        //}
                        //if (tmpPosSet.hole.Count() > 2)
                        //{
                        //    tmpPosSet.hole.RemoveAt(tmpPosSet.hole.Count - 1);
                        //}
                        //if (tmpPosSet.texture_coordinates.Count() > 2)
                        //{
                        //    tmpPosSet.texture_coordinates.RemoveAt(tmpPosSet.texture_coordinates.Count - 1);
                        //}
                    }

                    if (reader.LocalName == "LinearRing" ||
                    reader.LocalName == "Point" ||
                    reader.LocalName == "LineString")
                    {
                        PosBasedEntities.Add(tmpPosSet);
                    }

                    if (reader.LocalName.Equals("cellSpaceMember")
                        || reader.LocalName.Equals("GeneralSpace")
                        || reader.LocalName.Equals("TransitionSpace")
                        || reader.LocalName.Equals("cellSpaceBoundaryMember")
                        || reader.LocalName.Equals("transitionMember")
                        || reader.LocalName.Equals("stateMember"))
                    {
                        // Navi 모듈이 적용된 경우 cellSpaceMember 닫힘과 쌍이기 때문에
                        // 같은 ID 로 닫힘이 연속될 수 있다. 이런경우는 작업을 생략해도 됨.

                        if (mapCenter.ContainsKey(currentID) == false)
                        {
                            mapCenter.Add(currentID, localBounds.center);
                        }

                        localBounds = new Bounds();
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