using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesignLink.Types;
using DesignLink.Types.IO;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;

namespace DesignLink
{
    public class gsaInteraction
    {

        [MultiReturn(new[] { "GSA File" })]
        public static Dictionary<string, object> OpenGsaModel(string filePath)
        {

            Design aDesign = new Design();

            GsaDesignReader gsa = new GsaDesignReader();

            aDesign = gsa.Open(filePath);


            return new Dictionary<string, object>
                {
                    { "GSA File", aDesign },

                };
        }

        [MultiReturn(new[] { "GSA File","Point.GUID","GSA.Id","Point.isStructural", "Point.X", "Point.Y", "Point.Z" })]
        public static Dictionary<string, object> GetGsaNodes(Design GSAFile)
        {

            List<string> listGUID = new List<string>();
            List<string> listGSAid = new List<string>();
            List<double> listNodeX = new List<double>();
            List<double> listNodeY = new List<double>();
            List<double> listNodeZ = new List<double>();
            List<bool> isStructural = new List<bool>();

            foreach (Node thisNode in GSAFile.Nodes)
            {
                listGUID.Add(thisNode.GUID);
                listNodeX.Add(thisNode.Position.X);
                listNodeY.Add(thisNode.Position.Y);
                listNodeZ.Add(thisNode.Position.Z);

                StructNode thisStructNode = new StructNode();
                
                if (thisNode is StructNode)
                {
                    thisStructNode = (StructNode)thisNode;
                    isStructural.Add(true);
                    listGSAid.Add(thisStructNode.AppIDs.GetAppIdByAppName("GSA").ItemID);

                }

            }

            return new Dictionary<string, object>
                {
                    { "Point.GUID", listGUID },
                    { "GSA.Id", listGSAid },
                    { "Point.isStructural", isStructural },
                    { "Point.X", listNodeX },
                    { "Point.Y", listNodeY },
                    { "Point.Z", listNodeZ }
 
                };
        }

        [MultiReturn(new[] { "Elements.GUID", "Elements.GSAId", "Elements.Type","StartNode.Id", "Start.Point", "EndNode.Id", "End.Point", "SectionProfiles" })]
        public static Dictionary<string, object> GetGsaElements(Design GSAFile)
        {


            List<string> listGUID = new List<string>();
            List<string> listGSAId = new List<string>();
            List<string> list1dElementType = new List<string>();
            List<string> list1dStartNodeId = new List<string>();
            List<string> list1dEndNodeId = new List<string>();
            List<string> sectionProfiles = new List<string>();
            List<Point> list1StPoint = new List<Point>();
            List<Point> list1EndPoint = new List<Point>();

            foreach (Member thisMember in GSAFile.Members)
            {
                if (thisMember.HasStructuralRepresentation)
                {
                    foreach (StructAnalysisElement thisStructElem in thisMember.Representations.StructuralAnalysis)
                    {
                        listGUID.Add(thisStructElem.GUID);

                        AppID gsaId = new AppID();
                        StructAnalysis1DElement thisStruct1d = new StructAnalysis1DElement();


                        if (gsaId != null)
                        {
                            listGSAId.Add(thisStructElem.AppIDs.GetAppIdByAppName("GSA").ItemID);
                        }

                        if (thisStructElem is StructAnalysis1DElement)
                        {
                            //Process 1d type elements
                            thisStruct1d = (StructAnalysis1DElement)thisStructElem;
                            

                            //Get element type
                            list1dElementType.Add(thisStruct1d.ElementType.ToString());
                            
                            //Get Start Node GSA ID
                            gsaId = thisStruct1d.StartNode.AppIDs.GetAppIdByAppName("GSA");
                            if (gsaId != null)
                            {
                                list1dStartNodeId.Add(gsaId.ItemID);
                                list1StPoint.Add(Point.ByCoordinates(thisStruct1d.StartNode.Position.X, thisStruct1d.StartNode.Position.Y, thisStruct1d.StartNode.Position.Z));


                            }
                            //Get End Node GSA ID
                            gsaId = thisStruct1d.EndNode.AppIDs.GetAppIdByAppName("GSA");
                            if (gsaId != null)
                            {
                                list1dEndNodeId.Add(gsaId.ItemID);
                                list1EndPoint.Add(Point.ByCoordinates(thisStruct1d.EndNode.Position.X, thisStruct1d.EndNode.Position.Y, thisStruct1d.EndNode.Position.Z));

                            }

                            //Get Section Profiles Catalgoue Name
                            if (thisStruct1d.SectionProfile1 != null)
                            {
                                sectionProfiles.Add(thisStruct1d.SectionProfile1.CatalogueName);
                                
                            }

                        }

                        //else return: No 1d elements found!

                    }
                }
            }

            return new Dictionary<string, object>
                {
                    { "Elements.GUID", listGUID },
                    { "Elements.GSAId", listGSAId },
                    { "Elements.Type", list1dElementType },
                    { "StartNode.Id", list1dStartNodeId },
                    { "Start.Point", list1StPoint },
                    { "EndNode.Id", list1dEndNodeId },
                    { "End.Point", list1EndPoint },
                    { "SectionProfiles", sectionProfiles }

                };
        }



    };
}



