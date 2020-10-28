import sd
import os
import math
from sd.api.sdproperty import *
from sd.api.sdapplication import SDApplicationPath
from sd.api.sdresourcebitmap import SDResourceBitmap
from sd.api.sdtexture import SDTexture
from sd.api.sdresource import EmbedMethod
from sd.api.sdvaluearray import SDValueArray
from sd.api.sdtypeusage import SDTypeUsage
from sd.api.sdvalueusage import SDValueUsage
from sd.api.sdusage import SDUsage
from sd.api.sdvalueint import SDValueInt
from sd.api.sdvalueint2 import SDValueInt2
from sd.api.sdvaluefloat import SDValueFloat
from sd.api.sdvaluecolorrgba import SDValueColorRGBA
from sd.api.sdbasetypes import *
from sd.api.sbs.sdsbsarexporter import SDSBSARExporter

LabelList = ['Base Color', 'Normal', 'Roughness', 'Metallic', 'Ambient Occlusion', 'Height', 'Opacity']

ConnectionProList = {
'basecolor': 'basecolor',
'normal': 'normal', 
'roughness': 'roughness',
'metallic': 'metallic',
'height': 'height',
'opacity': 'opacity',
'ambient_occlusion': 'ambient_occlusion',
'ambientocclusion': 'ambient_occlusion'}


def createPBRRenderNode(app, pkgMgr, currentGraph):
    # create PBR Rendoer Node
	resourcePath = app.getPath(SDApplicationPath.DefaultResourcesDir)
	package = pkgMgr.loadUserPackage(os.path.join(resourcePath, 'packages', 'pbr_render_ortho.sbs'))
	packageName = 'PBR_render_ortho'
	instanceNode = currentGraph.newInstanceNode(package.findResourceFromUrl(packageName))
	instanceNode.setInputPropertyValueFromId('shape', SDValueInt.sNew(1))
	instanceNode.setInputPropertyValueFromId('envrionment_horizontal_rotation', SDValueFloat.sNew(0.06))
	instanceNode.setInputPropertyValueFromId('background_color', SDValueColorRGBA.sNew(ColorRGBA(0, 0, 0, 0)))
	instanceNode.setInputPropertyValueFromId('$outputsize', SDValueInt2.sNew(int2(-1,-1)))
	return instanceNode

def linkEnvironmentMap(app, pbrRenderNode, package, currentGraph):
    resourcePath = app.getPath(SDApplicationPath.DefaultResourcesDir)
    envMapPath = resourcePath + '/view3d/maps/glazed_patio.exr'
    envMap = SDResourceBitmap.sNewFromFile(package, envMapPath, EmbedMethod.Linked)
    envMapNode = currentGraph.newInstanceNode(envMap)
    envMapNode.newPropertyConnectionFromId('unique_filter_output', pbrRenderNode, 'environment_map')


def connectNode(materialNode, pbrRenderNode):
	inprops = materialNode.getProperties(SDPropertyCategory.Output)
	for prop in inprops:
		propId = prop.getId()
		label = prop.getLabel()
		#value = materialNode.getPropertyValue(prop)
		if label in LabelList:
			if propId in ConnectionProList:
				#print(propId)
				try:
					materialNode.newPropertyConnectionFromId(propId, pbrRenderNode, ConnectionProList[propId])
				except:
					pass
				else:
					print(propId + ' connected!')

def createOutputNode(currentGraph, pbrRenderNode):
    
    outputNode = currentGraph.newNode('sbs::compositing::output')
    sdValueArray = SDValueArray.sNew(SDTypeUsage.sNew(), 0)
    sdValueUsage = SDValueUsage.sNew(SDUsage.sNew('baseColor', 'RGBA', 'sRGB'))
    sdValueArray.pushBack(sdValueUsage)
    outputNode.setAnnotationPropertyValueFromId('usages', sdValueArray)
    pbrRenderNode.newPropertyConnectionFromId('basecolor', outputNode, 'inputNodeOutput')
    return outputNode


def lackthumbcheck(respath):
	sbsarpathlist = []
	count = 0
	for mainfolder in os.listdir(respath):
		mainpath = os.path.join(respath,mainfolder)
		for folder in os.listdir(mainpath):
			folderpath = os.path.join(mainpath,folder)
			folderflag = False
			for fileordir in os.listdir(folderpath):
				fileordirfull = os.path.join(folderpath,fileordir)
				if os.path.isdir(fileordirfull):
					if fileordir == "thumbnail":
						folderflag = True
			if not folderflag:
				sbsarflag = False
				for fileordir in os.listdir(folderpath):
					fileordirfull = os.path.join(folderpath,fileordir)
					if os.path.splitext(fileordir)[1] == '.sbsar':
						sbsarflag = True
						if fileordirfull not in sbsarpathlist:
							sbsarpathlist.append(fileordirfull)
							count +=1
				if not sbsarflag:
					for fileordir in os.listdir(folderpath):
						fileordirfull = os.path.join(folderpath,fileordir)
						if os.path.splitext(fileordir)[1] == '.sbs':
							sbsarpath = fileordirfull + 'ar'
							if sbsarpath not in sbsarpathlist:
								print("%s   ====>    %s" %(fileordirfull,sbsarpath))
								exportSbsToSbsar(fileordirfull,sbsarpath)
								sbsarpathlist.append(sbsarpath)
								count +=1
							
							
                
	return sbsarpathlist,count

def exportSbsToSbsar(respath, outpath):
	exporterInstance = SDSBSARExporter(ctx, None)
	e = exporterInstance.sNew()
	package = pkgMgr.loadUserPackage(respath)
	e.exportPackageToSBSAR(package, outpath)
	pkgMgr.unloadUserPackage(package)
    

ctx = sd.getContext()
app = ctx.getSDApplication()
UIMgr = app.getQtForPythonUIMgr()
currentGraph = UIMgr.getCurrentGraph()
#print(currentGraph)
pkgMgr = app.getPackageMgr()
currentpackage = pkgMgr.getUserPackages().getItem(0)
pbrRenderNode = createPBRRenderNode(app, pkgMgr, currentGraph)
linkEnvironmentMap(app, pbrRenderNode, currentpackage, currentGraph)
respath = r'E:\Temp\test'
pathlist,totalcount = lackthumbcheck(respath)
namelist = []
checkcount = 1



for file in pathlist:
	package = pkgMgr.loadUserPackage(file)
	resary = package.getChildrenResources(False)
	for res in resary:
		url = res.getUrl()
		instanceNode = currentGraph.newInstanceNode(package.findResourceFromUrl(url))
		connectNode(instanceNode,pbrRenderNode)
		outputNode = createOutputNode(currentGraph,pbrRenderNode)
		currentGraph.compute()
		prop = pbrRenderNode.getPropertyFromId('basecolor', SDPropertyCategory.Output)
		value = pbrRenderNode.getPropertyValue(prop)
		sdtexture = value.get()
		folderPath = file[0:file.rindex('\\')]
		if os.path.isdir(folderPath):
			thumbnailfolder = folderPath + "/thumbnail"
			if not os.path.exists(thumbnailfolder):
				os.makedirs(thumbnailfolder)
			MaterialPath = thumbnailfolder + file[file.rindex('\\'):file.rindex('.')] + '.png'
			if os.path.exists(MaterialPath):
				os.remove(MaterialPath)
			sdtexture.save(MaterialPath)
			print(MaterialPath + "      %d / %d " %(checkcount,totalcount))
			checkcount+=1
		currentGraph.deleteNode(instanceNode)
		currentGraph.deleteNode(outputNode)
		pkgMgr.unloadUserPackage(package)
	

