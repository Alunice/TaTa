using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest
{
    class FbxManager
    {
		const string FBXHeader = @"; FBX 7.4.0 project file
; ----------------------------------------------------

	FBXHeaderExtension:  {
		FBXHeaderVersion: 1003
		FBXVersion: 7400
		Creator: ""FBX SDK/FBX Plugins version 2020.1""
		SceneInfo: ""SceneInfo::GlobalInfo"", ""UserData"" {
		Type: ""UserData""
		Version: 100
		MetaData:  {
				Version: 100
				Title: ""Created by SH_AC Ouke""
				Subject: """"
				Author: ""Ouke""
				Keywords: ""Nodes Meshes Materials Textures Cameras Lights Skins Animation""
				Revision: ""1.0""
				Comment: """"
			}
		}
	}
	GlobalSettings:  {
		Version: 1000
		Properties70:  {
			P: ""UpAxis"", ""int"", ""Integer"", """",1
			P: ""UpAxisSign"", ""int"", ""Integer"", """",1
			P: ""FrontAxis"", ""int"", ""Integer"", """",2
			P: ""FrontAxisSign"", ""int"", ""Integer"", """",1
			P: ""CoordAxis"", ""int"", ""Integer"", """",0
			P: ""CoordAxisSign"", ""int"", ""Integer"", """",1
			P: ""OriginalUpAxis"", ""int"", ""Integer"", """",-1
			P: ""OriginalUpAxisSign"", ""int"", ""Integer"", """",1
			P: ""UnitScaleFactor"", ""double"", ""Number"", """",1
			P: ""OriginalUnitScaleFactor"", ""double"", ""Number"", """",1
			P: ""AmbientColor"", ""ColorRGB"", ""Color"", """",0,0,0
			P: ""DefaultCamera"", ""KString"", """", """", ""Producer Perspective""
			P: ""TimeMode"", ""enum"", """", """",0
			P: ""TimeProtocol"", ""enum"", """", """",2
			P: ""SnapOnFrameMode"", ""enum"", """", """",0
			P: ""TimeSpanStart"", ""KTime"", ""Time"", """",0
			P: ""TimeSpanStop"", ""KTime"", ""Time"", """",46186158000
			P: ""CustomFrameRate"", ""double"", ""Number"", """",-1
			P: ""TimeMarker"", ""Compound"", """", """"
			P: ""CurrentTimeMarker"", ""int"", ""Integer"", """",-1
		}
	}

; Documents Description
;------------------------------------------------------------------

Documents:  {
	Count: 1
	Document: 1669162400, ""Scene"", ""Scene"" {
		Properties70:  {
			P: ""SourceObject"", ""object"", """", """"
			P: ""ActiveAnimStackName"", ""KString"", """", """", """"
		}
		RootNode: 0
	}
}

; Document References
;------------------------------------------------------------------

References:  {
}

; Object definitions
;------------------------------------------------------------------

Definitions:  {
	Version: 100
	Count: 4
	ObjectType: ""GlobalSettings"" {
		Count: 1
	}
	ObjectType: ""Model"" {
		Count: 1
		PropertyTemplate: ""FbxNode"" {
			Properties70:  {
				P: ""QuaternionInterpolate"", ""enum"", """", """",0
				P: ""RotationOffset"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""RotationPivot"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""ScalingOffset"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""ScalingPivot"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""TranslationActive"", ""bool"", """", """",0
				P: ""TranslationMin"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""TranslationMax"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""TranslationMinX"", ""bool"", """", """",0
				P: ""TranslationMinY"", ""bool"", """", """",0
				P: ""TranslationMinZ"", ""bool"", """", """",0
				P: ""TranslationMaxX"", ""bool"", """", """",0
				P: ""TranslationMaxY"", ""bool"", """", """",0
				P: ""TranslationMaxZ"", ""bool"", """", """",0
				P: ""RotationOrder"", ""enum"", """", """",0
				P: ""RotationSpaceForLimitOnly"", ""bool"", """", """",0
				P: ""RotationStiffnessX"", ""double"", ""Number"", """",0
				P: ""RotationStiffnessY"", ""double"", ""Number"", """",0
				P: ""RotationStiffnessZ"", ""double"", ""Number"", """",0
				P: ""AxisLen"", ""double"", ""Number"", """",10
				P: ""PreRotation"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""PostRotation"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""RotationActive"", ""bool"", """", """",0
				P: ""RotationMin"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""RotationMax"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""RotationMinX"", ""bool"", """", """",0
				P: ""RotationMinY"", ""bool"", """", """",0
				P: ""RotationMinZ"", ""bool"", """", """",0
				P: ""RotationMaxX"", ""bool"", """", """",0
				P: ""RotationMaxY"", ""bool"", """", """",0
				P: ""RotationMaxZ"", ""bool"", """", """",0
				P: ""InheritType"", ""enum"", """", """",0
				P: ""ScalingActive"", ""bool"", """", """",0
				P: ""ScalingMin"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""ScalingMax"", ""Vector3D"", ""Vector"", """",1,1,1
				P: ""ScalingMinX"", ""bool"", """", """",0
				P: ""ScalingMinY"", ""bool"", """", """",0
				P: ""ScalingMinZ"", ""bool"", """", """",0
				P: ""ScalingMaxX"", ""bool"", """", """",0
				P: ""ScalingMaxY"", ""bool"", """", """",0
				P: ""ScalingMaxZ"", ""bool"", """", """",0
				P: ""GeometricTranslation"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""GeometricRotation"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""GeometricScaling"", ""Vector3D"", ""Vector"", """",1,1,1
				P: ""MinDampRangeX"", ""double"", ""Number"", """",0
				P: ""MinDampRangeY"", ""double"", ""Number"", """",0
				P: ""MinDampRangeZ"", ""double"", ""Number"", """",0
				P: ""MaxDampRangeX"", ""double"", ""Number"", """",0
				P: ""MaxDampRangeY"", ""double"", ""Number"", """",0
				P: ""MaxDampRangeZ"", ""double"", ""Number"", """",0
				P: ""MinDampStrengthX"", ""double"", ""Number"", """",0
				P: ""MinDampStrengthY"", ""double"", ""Number"", """",0
				P: ""MinDampStrengthZ"", ""double"", ""Number"", """",0
				P: ""MaxDampStrengthX"", ""double"", ""Number"", """",0
				P: ""MaxDampStrengthY"", ""double"", ""Number"", """",0
				P: ""MaxDampStrengthZ"", ""double"", ""Number"", """",0
				P: ""PreferedAngleX"", ""double"", ""Number"", """",0
				P: ""PreferedAngleY"", ""double"", ""Number"", """",0
				P: ""PreferedAngleZ"", ""double"", ""Number"", """",0
				P: ""LookAtProperty"", ""object"", """", """"
				P: ""UpVectorProperty"", ""object"", """", """"
				P: ""Show"", ""bool"", """", """",1
				P: ""NegativePercentShapeSupport"", ""bool"", """", """",1
				P: ""DefaultAttributeIndex"", ""int"", ""Integer"", """",-1
				P: ""Freeze"", ""bool"", """", """",0
				P: ""LODBox"", ""bool"", """", """",0
				P: ""Lcl Translation"", ""Lcl Translation"", """", ""A"",0,0,0
				P: ""Lcl Rotation"", ""Lcl Rotation"", """", ""A"",0,0,0
				P: ""Lcl Scaling"", ""Lcl Scaling"", """", ""A"",1,1,1
				P: ""Visibility"", ""Visibility"", """", ""A"",1
				P: ""Visibility Inheritance"", ""Visibility Inheritance"", """", """",1
			}
		}
	}
	ObjectType: ""Geometry"" {
		Count: 1
		PropertyTemplate: ""FbxMesh"" {
			Properties70:  {
				P: ""Color"", ""ColorRGB"", ""Color"", """",0.8,0.8,0.8
				P: ""BBoxMin"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""BBoxMax"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""Primary Visibility"", ""bool"", """", """",1
				P: ""Casts Shadows"", ""bool"", """", """",1
				P: ""Receive Shadows"", ""bool"", """", """",1
			}
		}
	}
	ObjectType: ""Material"" {
		Count: 1
		PropertyTemplate: ""FbxSurfaceLambert"" {
			Properties70:  {
				P: ""ShadingModel"", ""KString"", """", """", ""Lambert""
				P: ""MultiLayer"", ""bool"", """", """",0
				P: ""EmissiveColor"", ""Color"", """", ""A"",0,0,0
				P: ""EmissiveFactor"", ""Number"", """", ""A"",1
				P: ""AmbientColor"", ""Color"", """", ""A"",0.2,0.2,0.2
				P: ""AmbientFactor"", ""Number"", """", ""A"",1
				P: ""DiffuseColor"", ""Color"", """", ""A"",0.8,0.8,0.8
				P: ""DiffuseFactor"", ""Number"", """", ""A"",1
				P: ""Bump"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""NormalMap"", ""Vector3D"", ""Vector"", """",0,0,0
				P: ""BumpFactor"", ""double"", ""Number"", """",1
				P: ""TransparentColor"", ""Color"", """", ""A"",0,0,0
				P: ""TransparencyFactor"", ""Number"", """", ""A"",0
				P: ""DisplacementColor"", ""ColorRGB"", ""Color"", """",0,0,0
				P: ""DisplacementFactor"", ""double"", ""Number"", """",1
				P: ""VectorDisplacementColor"", ""ColorRGB"", ""Color"", """",0,0,0
				P: ""VectorDisplacementFactor"", ""double"", ""Number"", """",1
			}
		}
	}
}";

		const string OBJProperties = @"; Object properties
;------------------------------------------------------------------";

		const string OBJConnection = @"; Object connections
;------------------------------------------------------------------";

		const string MaterialHash = "1737697776";

		const string MaterialElement = @"    Material: 1737697776, ""Material::Default_Material"", """" {
		Version: 102
		ShadingModel: ""lambert""
		MultiLayer: 0
		Properties70:  {
			P: ""AmbientColor"", ""Color"", """", ""A"",0,0,0
			P: ""DiffuseColor"", ""Color"", """", ""A"",1,1,1
			P: ""Emissive"", ""Vector3D"", ""Vector"", """",0,0,0
			P: ""Ambient"", ""Vector3D"", ""Vector"", """",0,0,0
			P: ""Diffuse"", ""Vector3D"", ""Vector"", """",1,1,1
			P: ""Opacity"", ""double"", ""Number"", """",1
		}
	}";

		const string MaterialLayer = @"        LayerElementMaterial: 0 {
			Version: 101
			Name: ""Material""
			MappingInformationType: ""AllSame""
			ReferenceInformationType: ""IndexToDirect""
			Materials: *1 {
				a: 0
			} 
		}";

		List<string[]> _datas = new List<string[]>();
		List<FbxVertexData> indexDataList = new List<FbxVertexData>();
		Dictionary<int, int> _indexPair = new Dictionary<int, int>();
		List<int> unmergedPolygons = new List<int>();

		bool hasNormal = false;
		bool hasVertexColor = false;
		bool hasUV0 = false;
		bool hasUV1 = false;
		bool hasUV2 = false;
		bool hasUV3 = false;


		bool hasBinormal = false;
		bool hasTangent = false;

		string COL_POS_NAME = "POSITION0.x";
		string COL_NM_NAME = "NORMAL0.x";
		string COL_UV0_NAME = "TEXCOORD0.x";
		string COL_UV1_NAME = "TEXCOORD1.x";
		string COL_UV2_NAME = "TEXCOORD2.x";
		string COL_UV3_NAME = "TEXCOORD3.x";
		string COL_VETCOLOR_NAME = "COLOR.x";


		private string _modelHash = "";
		private string _sceneHash = "";
		private string _modelName = "";

		CSVToObj _formObj = null;
		public void SetFormOBJ(CSVToObj obj)
		{
			_formObj = obj;
		}


		public void SetColName(string posName,string nmName,string uvName,
			string uv1Name,string uv2Name, string uv3Name, string vertexColorName)
		{
			COL_POS_NAME = posName;
			COL_NM_NAME = nmName;
			COL_UV0_NAME = uvName;

			COL_UV1_NAME = uv1Name;
			COL_UV2_NAME = uv2Name;
			COL_UV3_NAME = uv3Name;
			COL_VETCOLOR_NAME = vertexColorName;

			hasNormal = (COL_NM_NAME == "" || COL_NM_NAME == " NULL") ? false : true;
			hasUV0 = (COL_UV0_NAME == "" || COL_UV0_NAME == " NULL") ? false : true;
			hasUV1 = (COL_UV1_NAME == "" || COL_UV1_NAME == " NULL") ? false : true;
			hasUV2 = (COL_UV2_NAME == "" || COL_UV2_NAME == " NULL") ? false : true;
			hasUV3 = (COL_UV3_NAME == "" || COL_UV3_NAME == " NULL") ? false : true;
			hasVertexColor = (COL_VETCOLOR_NAME == "" || COL_VETCOLOR_NAME == " NULL") ? false : true;
		}

		public void BuildFBXData(List<string[]> origDatas,string outputPath)
		{

			_formObj.AddListViewItems("prepare fbx data!!!(1/4)");
			PretreatmentDatas(origDatas, outputPath);
			StringBuilder builder = new StringBuilder();
			builder.Append(FBXHeader);
			builder.AppendLine(string.Empty);
			_formObj.AddListViewItems("Build Obj Properties!!!(2/4)");
			BuildObjProperties(builder, _modelHash, _sceneHash, _modelName);
			_formObj.AddListViewItems("Build Obj Connection!!!(3/4)");
			BuildObjConnection(builder, _modelHash, _sceneHash, _modelName);
			StreamWriter sw = new StreamWriter(outputPath, false);
			sw.Write(builder);
			sw.Close();
			_formObj.AddListViewItems("success!!!(4/4)");
			string consoleMassage = "vertex count: " + indexDataList.Count + ",face count : " + unmergedPolygons.Count / 3;
			_formObj.AddListViewItems(consoleMassage);
			_formObj.AddListViewItems("Output Path is : " + outputPath);
		}

		private void PretreatmentDatas(List<string[]> origDatas, string outputPath)
		{
			_datas = origDatas;
			if (_datas.Count < 1)
			{
				MessageBox.Show("CSV文件的数据有错误");
				throw new Exception("CSV文件的数据有错误");
			}
			var titleDatas = _datas[0];
			int dataLenth = titleDatas.Length;
			int posXColumn = 0, normalXColumn = 0, uvXColumn = 0;
			int uv1XColumn = 0, uv2XColumn = 0, uv3XColumn = 0, verColorColumn = 0;

			for (int i = 0; i < dataLenth; i++)
			{
				string title = titleDatas[i];
				if (title.Contains(COL_POS_NAME))
				{
					posXColumn = i;
				}
				else if (title.Contains(COL_UV0_NAME))
				{
					uvXColumn = i;
				}
				else if (title.Contains(COL_NM_NAME))
				{
					normalXColumn = i;
				}
				else if (title.Contains(COL_UV1_NAME))
				{
					uv1XColumn = i;
				}
				else if (title.Contains(COL_UV2_NAME))
				{
					uv2XColumn = i;
				}
				else if (title.Contains(COL_UV3_NAME))
				{
					uv3XColumn = i;
				}
				else if (title.Contains(COL_VETCOLOR_NAME))
				{
					verColorColumn = i;
				}
			}

			for (int i = 1; i < _datas.Count; i++)
			{
				var tempDatas = _datas[i];
				for (int ii = 0; ii < dataLenth; ii++)
				{
					if (tempDatas[ii].Contains("E"))
					{
						tempDatas[ii] = "0";
					}
				}
			}
			int[] orderArray = { 0, 2, 1 };
			for(int i =1; i < _datas.Count-2; i += 3)
			{
				// build index buffer data
				for(int j = 0;j < orderArray.Length; j++)
				{
					int index = i + orderArray[j];
					var tempDatas = _datas[index];
					int id = int.Parse(tempDatas[1]);
					unmergedPolygons.Add(id);
					if (!_indexPair.ContainsKey(id))
					{
						//normal and pos multiply -1 since MAYA and RenderDoc coord diffs
						FbxVertexData newData = new FbxVertexData(
							float.Parse(tempDatas[posXColumn]) * -1f,     //pos.x
							float.Parse(tempDatas[posXColumn + 1]),     //pos.y
							float.Parse(tempDatas[posXColumn + 2]),     //pos.z
							float.Parse(tempDatas[posXColumn + 3]),     //pos.w
							float.Parse(tempDatas[normalXColumn]) * -1f,     //nor.x
							float.Parse(tempDatas[normalXColumn + 1]),     //nor.y
							float.Parse(tempDatas[normalXColumn + 2]),     //nor.z
							float.Parse(tempDatas[uvXColumn]),     //u0
							float.Parse(tempDatas[uvXColumn + 1]),     //v0
							float.Parse(tempDatas[uv1XColumn]),     //u1
							float.Parse(tempDatas[uv1XColumn + 1]),     //v1
							float.Parse(tempDatas[uv2XColumn]),     //u2
							float.Parse(tempDatas[uv2XColumn + 1]),     //v2
							float.Parse(tempDatas[uv3XColumn]),     //u3
							float.Parse(tempDatas[uv3XColumn + 1]),     //v3
							float.Parse(tempDatas[verColorColumn]),     //r
							float.Parse(tempDatas[verColorColumn + 1]),     //g
							float.Parse(tempDatas[verColorColumn + 2]),     //b
							float.Parse(tempDatas[verColorColumn + 3])     //a
							);
						_indexPair.Add(id, indexDataList.Count);
						indexDataList.Add(newData);
					}
				}	
			}

			string modelName = outputPath.Substring(outputPath.LastIndexOf('\\')+1, outputPath.LastIndexOf('.') - outputPath.LastIndexOf('\\')-1);
			_modelName = modelName;
			_modelHash = modelName.GetHashCode() + "";
			_sceneHash = (modelName + "Scene").GetHashCode() + "";

		}

		private void BuildObjProperties(StringBuilder builder, string ModelHash
			, string SceneHash,string ModelName)
		{
			builder.Append(OBJProperties);
			builder.AppendLine(string.Empty);

			builder.Append("Objects:  {\r\n");
			_formObj.AddListViewItems("    Build Obj Properties(Geometry Element)!!!");
			BuildGeometryElement(builder, SceneHash);
			_formObj.AddListViewItems("    Build Obj Properties(Model Element)!!!");
			BuildModelElement(builder, ModelHash, ModelName);
			BuildMaterialElement(builder);
			builder.Append("}\r\n");

		}

		
		private void BuildGeometryElement(StringBuilder builder,string SceneHash)
		{
			int unmergeCount = unmergedPolygons.Count;
			int vertexCount = indexDataList.Count;

			builder.Append("    Geometry: " + SceneHash + ", \"Geometry::Scene\", \"Mesh\" { \r\n");
			BuildVerticesSub(builder, unmergeCount, vertexCount);

			if(hasNormal)
				BuildNormalSub(builder, unmergeCount);

			if (hasBinormal)
				BuildBinormalSub(builder, unmergeCount);

			if (hasTangent)
				BuildTangentSub(builder, unmergeCount);

			if (hasVertexColor)
				BuildVertexColorSub(builder, unmergeCount);

			if (hasUV0)
				BuildUVSub(builder, unmergeCount, vertexCount, 0);

			if (hasUV1)
				BuildUVSub(builder, unmergeCount, vertexCount, 1);
			if (hasUV2)
				BuildUVSub(builder, unmergeCount, vertexCount, 2);
			if (hasUV3)
				BuildUVSub(builder, unmergeCount, vertexCount, 3);

			BuildMaterialLayerSub(builder);
			BuildLayerSub(builder);
			BuildOtherLayerSub(builder);


			builder.Append("    }");
			builder.AppendLine(string.Empty);
		}

		private void BuildVerticesSub(StringBuilder builder,int unmergeCount,int vertexCount)
		{
			string positionString = "";
			for(int i = 0; i < vertexCount; i++)
			{
				if (i != 0)
					positionString += ",";
				var vertData = indexDataList[i];
				positionString += vertData.posX + "," + vertData.posY + "," + vertData.posZ;
			}
			string vertexString = $"        Vertices: *{vertexCount * 3} " + "{\r\n" +
				"        a: " + positionString + "\r\n" +
				"        }";

			builder.Append(vertexString);
			builder.AppendLine(string.Empty);

			string polygonString = "";
			for (int i = 0; i < unmergeCount; i+=3)
			{
				if (i != 0)
					polygonString += ",";
				var vertData1 = _indexPair[unmergedPolygons[i]];
				var vertData2 = _indexPair[unmergedPolygons[i+1]] ;
				var vertData3 = _indexPair[unmergedPolygons[i+2]] ;
				// unity to maya index order form 0-1-2 to 0-2-1
				vertData3 = -vertData3 - 1;
				polygonString += vertData1 + "," + vertData2 + "," + vertData3;
			}
			string polygonVertexString = $"        PolygonVertexIndex: *{unmergeCount} " + "{\r\n" +
				"        a: " + polygonString + "\r\n" +
				"        }";

			builder.Append(polygonVertexString);
			builder.AppendLine(string.Empty);

			builder.Append("        GeometryVersion: 124");
			builder.AppendLine(string.Empty);
		}

		private void BuildNormalSub(StringBuilder builder, int unmergeCount)
		{
			string normalTitle = @"        LayerElementNormal: 0 {
            Version: 102
			Name: ""Normals""
			MappingInformationType: ""ByPolygonVertex""
			ReferenceInformationType: ""Direct""";
			builder.Append(normalTitle);
			builder.AppendLine(string.Empty);

			string noramlString = "";
			for (int i = 0; i < unmergeCount; i++)
			{
				if (i != 0)
					noramlString += ",";
				int index = _indexPair[unmergedPolygons[i]];
				var vertData = indexDataList[index];
				noramlString += vertData.normalX + "," + vertData.normalY + "," + vertData.normalZ;
			}
			string vertexNormalString = $"        Normals: *{unmergeCount * 3} " + "{\r\n" +
				"        a: " + noramlString + "\r\n" +
				"        }";

			builder.Append(vertexNormalString);
			builder.AppendLine(string.Empty);

			string tempNW = "";
			for(int i = 0; i< unmergeCount; i++)
			{
				if (i != 0)
					tempNW += ",";
				tempNW += "1";
			}

			string normalW = "            NormalsW: *" + unmergeCount + " {\r\n" + 
				"                a: "+ tempNW + "\r\n" +
				"            }";

			builder.Append(normalW);
			builder.AppendLine(string.Empty);
			builder.Append("        }");
			builder.AppendLine(string.Empty);
		}

		private void BuildBinormalSub(StringBuilder builder, int unmergeCount)
		{

		}

		private void BuildTangentSub(StringBuilder builder, int unmergeCount)
		{

		}

		private void BuildVertexColorSub(StringBuilder builder, int unmergeCount)
		{
			string vcTitle = @"        LayerElementColor: 0 {
			Version: 101
			Name: ""VertexColors""
			MappingInformationType: ""ByPolygonVertex""
			ReferenceInformationType: ""IndexToDirect""";
			vcTitle += "\r\n";

			string colorString = "";
			for (int i = 0; i < unmergeCount; i++)
			{
				if (i != 0)
					colorString += ",";
				var vertData = indexDataList[_indexPair[unmergedPolygons[i]]];
				colorString += vertData.ColorR + "," + vertData.ColorG + ","+ vertData.ColorB + ","+ vertData.ColorA;
			}
			string vertexColorString = $"            Colors: *{unmergeCount *4} " + "{\r\n" +
				"                a: " + colorString + "\r\n" +
				"            }\r\n";
			vcTitle += vertexColorString;


			string polygonString = "";
			for (int i = 0; i < unmergeCount; i++)
			{
				if (i != 0)
					polygonString += ",";
				var vertData = _indexPair[unmergedPolygons[i]] ;
				polygonString += vertData;
			}
			string polygonVertexString = $"            ColorIndex: *{unmergeCount} " + "{\r\n" +
				"                a: " + polygonString + "\r\n" +
				"            }\r\n";

			vcTitle += polygonVertexString;
			vcTitle += "        }";
			builder.Append(vcTitle);
			builder.AppendLine(string.Empty);
		}

		private void BuildUVSub(StringBuilder builder, int unmergeCount,int vertexCount, int uvSet)
		{
			string uvString = "        LayerElementUV: " + uvSet + " {\r\n";
			uvString += "            Version: 101\r\n";
			uvString += "            Name: \"UVSet" + uvSet + "\"\r\n";
			uvString += "            MappingInformationType: \"ByPolygonVertex\"\r\n";
			uvString += "            ReferenceInformationType: \"IndexToDirect\"\r\n";
			string tempUVString = "";
			for (int i = 0; i < vertexCount; i++)
			{
				if (i != 0)
					tempUVString += ",";
				var vertData = indexDataList[i];
				float x_value, y_value;
				if(uvSet == 0)
				{
					x_value = vertData.uvX; y_value = vertData.uvY;
				}else if( uvSet == 1)
				{
					x_value = vertData.uv1X; y_value = vertData.uv1Y;
				}
				else if (uvSet == 2)
				{
					x_value = vertData.uv2X; y_value = vertData.uv2Y;
				}
				else
				{
					x_value = vertData.uv3X; y_value = vertData.uv3Y;
				}

				tempUVString += x_value + "," + y_value;
			}
			string vertexUVString = $"            UV: *{vertexCount * 2} " + "{\r\n" +
				"                a: " + tempUVString + "\r\n" +
				"            }\r\n";
			uvString += vertexUVString;

			string polygonString = "";
			for (int i = 0; i < unmergeCount; i++)
			{
				if (i != 0)
					polygonString += ",";
				var vertData = _indexPair[unmergedPolygons[i]];
				polygonString += vertData;
			}
			string polygonVertexString = $"            UVIndex: *{unmergeCount} " + "{\r\n" +
				"                a: " + polygonString + "\r\n" +
				"            }";

			uvString += polygonVertexString;

			uvString += "        }\r\n";

			builder.Append(uvString);
		}

		private void BuildMaterialLayerSub(StringBuilder builder)
		{
			builder.Append(MaterialLayer);
			builder.AppendLine(string.Empty);
		}

		private void BuildLayerSub(StringBuilder builder)
		{
			string layerString = @"        Layer: 0 {
			Version: 100";
			layerString += "\r\n";
			if (hasNormal)
			{
				layerString += @"            LayerElement:  {
				Type: ""LayerElementNormal""
				TypedIndex: 0
			}";
				layerString += "\r\n";
			}

			if (hasBinormal)
			{
				layerString += @"            LayerElement:  {
				Type: ""LayerElementBinormal""
				TypedIndex: 0
			}";
				layerString += "\r\n";
			}

			if (hasTangent)
			{
				layerString += @"            LayerElement:  {
				Type: ""LayerElementBinormal""
				TypedIndex: 0
			}";
				layerString += "\r\n";
			}
			if (hasUV0)
			{
				layerString += @"            LayerElement:  {
				Type: ""LayerElementUV""
				TypedIndex: 0
			}";
				layerString += "\r\n";
			}

			if (hasVertexColor)
			{
				layerString += @"            LayerElement:  {
				Type: ""LayerElementColor""
				TypedIndex: 0
			}";
				layerString += "\r\n";
			}

			layerString += @"            LayerElement:  {
				Type: ""LayerElementMaterial""
				TypedIndex: 0
			}";
			layerString += "\r\n";

			layerString += "        }";
			builder.Append(layerString);
			builder.AppendLine(string.Empty);
		}

		private void BuildOtherLayerSub(StringBuilder builder)
		{
			string OtherLayerTitle = @"        Layer: replaceIndex {
			Version: 100
			LayerElement:  {
				Type: ""LayerElementUV""
				TypedIndex: replaceIndex
			}
		}";
			if (hasUV1)
			{
				builder.Append(OtherLayerTitle.Replace("replaceIndex", "1"));
				builder.AppendLine(string.Empty);
			}
			if (hasUV2)
			{
				builder.Append(OtherLayerTitle.Replace("replaceIndex", "2"));
				builder.AppendLine(string.Empty);
			}
			if (hasUV3)
			{
				builder.Append(OtherLayerTitle.Replace("replaceIndex", "3"));
				builder.AppendLine(string.Empty);
			}
		}

		private void BuildModelElement(StringBuilder builder,string ModelHash,string ModelName)
		{
			string ModelString = $"   Model: {ModelHash}, \"Model::{ModelName}\", \"Mesh\" " +
				@"{
		Version: 232
		Properties70:  {
			P: ""InheritType"", ""enum"", """", """",1
			P: ""ScalingMax"", ""Vector3D"", ""Vector"", """",0,0,0
			P: ""DefaultAttributeIndex"", ""int"", ""Integer"", """",0
		}
		Shading: W
		Culling: ""CullingOff""
	}";
			builder.Append(ModelString);
			builder.AppendLine(string.Empty);
		}

		private void BuildMaterialElement(StringBuilder builder)
		{
			builder.Append(MaterialElement);
			builder.AppendLine(string.Empty);
		}

		private void BuildObjConnection(StringBuilder builder,string ModelHash
			,string SceneHash,string ModelName)
		{
			builder.Append(OBJConnection);
			builder.AppendLine(string.Empty);
			string ConneciontString = "Connections:  {\r\n" +
				$"    ;Model::{ModelName}, Model::RootNode\r\n" +
				$"    C: \"OO\",{ModelHash},0\r\n" +
				$"    ;Material::Default_Material, Model::{ModelName}\r\n" +
				$"    C: \"OO\",{MaterialHash},{ModelHash}\r\n" +
				$"    ;Geometry::Scene, Model::{ModelName}\r\n" +
				$"    C: \"OO\",{SceneHash},{ModelHash}\r\n" +
				"}";
			builder.Append(ConneciontString);
			builder.AppendLine(string.Empty);
		}

		public struct FbxVertexData
		{
			public float posX;
			public float posY;
			public float posZ;
			public float posW;

			public float normalX;
			public float normalY;
			public float normalZ;

			public float uvX;
			public float uvY;

			public float uv1X;
			public float uv1Y;
			public float uv2X;
			public float uv2Y;
			public float uv3X;
			public float uv3Y;

			public float ColorR;
			public float ColorG;
			public float ColorB;
			public float ColorA;

			public FbxVertexData( float posX, float posY, float posZ, float posW, float normalX, float normalY, float normalZ, float uvX, float uvY,
			float uv1X, float uv1Y, float uv2X, float uv2Y, float uv3X, float uv3Y, float r, float g, float b, float a  )
			{
				this.posX = posX;
				this.posY = posY;
				this.posZ = posZ;
				this.posW = posW;
				this.normalX = normalX;
				this.normalY = normalY;
				this.normalZ = normalZ;
				this.uvX = uvX;
				this.uvY = uvY;

				this.uv1X = uv1X;
				this.uv1Y = uv1Y; 
				this.uv2X = uv2X;
				this.uv2Y = uv2Y;
				this.uv3X = uv3X;
				this.uv3Y = uv3Y;

				this.ColorR = r;
				this.ColorG = g;
				this.ColorB = b;
				this.ColorA = a;
			}
		}
	}


}
