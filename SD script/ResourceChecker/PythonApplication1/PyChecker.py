import zipfile
import os,shutil
import cv2
import math
import numpy as np
from PIL import Image,ImageFilter,ImageFile
Image.MAX_IMAGE_PIXELS = None
ImageFile.LOAD_TRUNCATED_IMAGES = True

print(os.getcwd())

type = 'png'
thumbnailsize = 512
fileFolder = "E:/Temp/res/poliigon/"

'''
already done 
CGAxis all files
Real Displacement Textures Collection all files

'''

_pbrfilelist = ['_Asphalt','_Brick Walls','_Concrete','_Fabrics','_Leather',
                '_Marble','_Metals','_Parquets','_Pavements','_Roads',
                '_Rocks','_Roofs','_Snow','_Soil','_Stones',
                '_Wall','_Wood','_Wooden Tiles']

cgaxis1 = [
           'Real Displacement Textures Collection 2'
           ]

def resetfolderImgsize(path):
    for jpg in os.listdir(path):
        subfullpath = os.path.join(jpg,path)
        if not os.path.isdir(subfullpath) and os.path.splitext(jpg)[1] == '.jpg' or os.path.splitext(jpg)[1] == '.png':
            ResizeImage(subfullpath,subfullpath,2048,2048,type)

def ResizeThumbnailImage(filein,fileout,maxValue):
    im = Image.open(filein)
    coefficient = maxValue / max(im.size)
    coefficient = min(coefficient,1)
    w = math.ceil(im.size[0] * coefficient)
    h = math.ceil(im.size[1] * coefficient)
    out = im.resize((w, h),Image.ANTIALIAS)
    out.save(fileout,type)

def ResizeThumbnailImageTif(filein,fileout,maxValue):
    im = Image.open(filein)
    im.save(fileout)
    im = Image.open(fileout)
    coefficient = maxValue / max(im.size)
    coefficient = min(coefficient,1)
    w = math.ceil(im.size[0] * coefficient)
    h = math.ceil(im.size[1] * coefficient)
    out = im.resize((w, h),Image.ANTIALIAS)
    out.save(fileout,type)

def ResizeThumbnailImageExr(filein,fileout,maxValue):
    im = cv2.imread(filein,cv2.IMREAD_UNCHANGED)
    im = im * 65535
    im[im>65535] = 65535
    im = np.uint16(im)
    cv2.imwrite(fileout,im)
    im = Image.open(fileout)
    coefficient = maxValue / max(im.size)
    coefficient = min(coefficient,1)

    w = math.ceil(im.size[0] * coefficient)
    h = math.ceil(im.size[1] * coefficient)

    out = im.resize((w, h),Image.ANTIALIAS)
    out.save(fileout,type)



def createFolder(respath):
    previewpath = respath + "/Preview"
    outdir =  "E:/Temp/test"
    for file in os.listdir(previewpath):
         filepath = os.path.join(previewpath,file)
         foldername = os.path.splitext(file)[0]
         outpath = outdir + "/Poliigon-" + foldername
         if not os.path.exists(outpath):
            os.makedirs(outpath)
            print("" + outpath,flush = True)
         if not os.path.exists(outpath + "/thumbnail") :
            os.makedirs(outpath + "/thumbnail")
         jpgoutpath = outpath + "/thumbnail/" + file
         print("        " + file,flush = True)
         ResizeThumbnailImage(filepath,jpgoutpath,thumbnailsize)
         jpgfolder = respath + "/" +  foldername
         if os.path.exists(jpgfolder):
             for jpg in os.listdir(jpgfolder):
                 if os.path.splitext(jpg)[1] == '.jpg' or os.path.splitext(jpg)[1] == '.png':
                     jpgpath = os.path.join(jpgfolder,jpg)
                     jpgoutpath = outpath + "/" + jpg
                     print("        " + jpg,flush = True)
                     ResizeThumbnailImage(jpgpath,jpgoutpath,2048)
                  


def createpolifabric(respath):
    outdir =  "E:/Temp/test"
    for folder in os.listdir(respath):
        fullfolderpath = os.path.join(respath,folder)
        for subfolder in os.listdir(fullfolderpath):
            subfolderfullpath = os.path.join(fullfolderpath,subfolder)
            outpath = outdir + "/" + folder
            if not os.path.exists(outpath) :
                os.makedirs(outpath)
            if subfolder.find("Previews") != -1:
                if not os.path.exists(outpath + "/thumbnail") :
                    os.makedirs(outpath + "/thumbnail")
                for jpg in os.listdir(subfolderfullpath):
                    if os.path.splitext(jpg)[1] == '.jpg' or os.path.splitext(jpg)[1] == '.png':
                        jpgpath = os.path.join(subfolderfullpath,jpg)
                        jpgoutpath = outpath + "/thumbnail/" + jpg
                        print("        " + jpg,flush = True)
                        ResizeThumbnailImage(jpgpath,jpgoutpath,thumbnailsize)
            else:
                for root,dirs,files in os.walk(subfolderfullpath):
                    for jpg in files:
                        if os.path.splitext(jpg)[1] == '.jpg' or os.path.splitext(jpg)[1] == '.png':
                            jpgpath = os.path.join(root,jpg)
                            jpgoutpath = outpath + "/" + jpg
                            print("        " + jpg,flush = True)
                            ResizeThumbnailImage(jpgpath,jpgoutpath,2048)


def spmakerforMegascan(respath):
    outdir =  "E:/Temp/test"
    totalcount = 2478
    index = 1
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        outfolder = outdir + "/" + file
        if not os.path.exists(outfolder):
            os.makedirs(outfolder)
            os.makedirs(outfolder + "/thumbnail")
        if os.path.isdir(filepath):
            print(filepath + "     %d/%d"%(index,totalcount) )
            index +=1
            if os.path.exists(filepath + "/previews"):
                for jpg in os.listdir(filepath + "/previews"):
                    jpgpath = os.path.join(filepath + "/previews",jpg)
                    if os.path.splitext(jpg)[1] == '.jpg' or os.path.splitext(jpg)[1] == '.png':
                        jpgout = jpg.replace("8K","2K").replace("4K","2K")
                        jpgoutpath = os.path.join(outfolder + "/thumbnail",jpgout)
                        print("     "+ jpg )
                        ResizeThumbnailImage(jpgpath,jpgoutpath,512)


            for jpgordir in os.listdir(filepath):
                jpgordirpath = os.path.join(filepath,jpgordir)
                if not os.path.isdir(jpgordirpath):
                    if os.path.splitext(jpgordir)[1] == '.jpg' or os.path.splitext(jpgordir)[1] == '.png':
                        jpgout = jpgordir.replace("8K","2K").replace("4K","2K")
                        jpgoutpath = os.path.join(outfolder,jpgout)
                        imgsize = 2048
                        if jpgordir.find("Preview") != -1:
                            jpgoutpath = os.path.join(outfolder + "/thumbnail",jpgout)
                            imgsize = 512
                        print("     "+ jpgordir )
                        ResizeThumbnailImage(jpgordirpath,jpgoutpath,imgsize)
                   
                    elif os.path.splitext(jpgordir)[1] == '.exr': 
                        pngout = jpgordir.replace("exr","png")
                        pngoutpath = os.path.join(outfolder,pngout)
                        print("     "+ jpgordir )
                        ResizeThumbnailImageExr(jpgordirpath,pngoutpath,2048)
 


def movefolderallfiles(respath,outdir):
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        outpath = outdir + "/" + file
        if os.path.isdir(filepath):
            if not os.path.exists(outpath):
                os.makedirs(outpath)
            movefolderallfiles(filepath,outpath)
        else:
            shutil.copy(filepath,outpath)


def fuckfolder(respath):
    outdir =  "E:/Temp/zzr"
    resfullpath = "E:/Temp/res/Substance/_Collection_Free/SignatureMaterials_by_DanielThiger/" + respath
    for root,dirs,files in os.walk(resfullpath):
        for jpg in files:  
            jpgpath = os.path.join(root,jpg)
            foldername ="SBSource-" + os.path.splitext(jpg)[0]
            outfolder = outdir + "/" + foldername
            jpgoutpath = outfolder + "/SBSource-" + jpg
            if not os.path.exists(outfolder):
                os.makedirs(outfolder)
                print("    " + foldername,flush = True)
            shutil.copy(jpgpath,jpgoutpath)
            print("        " + jpg,flush = True)

def spformodel(respath):
    for file in os.listdir(respath):
        fullpath = os.path.join(respath,file)
        if os.path.isdir(fullpath):
            for subfile in os.listdir(fullpath):
                print()


def makedir(foldername,index):
    outdir =  "E:/Temp/zzr/"
    outdir2 = "E:/Temp/test/"
    if index == 1 :
        os.makedirs(outdir + foldername)
        os.makedirs(outdir + foldername +"/thumbnail")
        os.makedirs(outdir + foldername +"/textures")
    else:
        os.makedirs(outdir2+ foldername)
        os.makedirs(outdir2 + foldername +"/thumbnail")
        os.makedirs(outdir2 + foldername +"/textures")

def sbsourcemaker(respath):
    outdir = "E:/Temp/test/"
    for subdir in os.listdir(respath):
        subdirpath = os.path.join(respath,subdir)
        if os.path.isdir(subdirpath):
            if not os.path.exists(outdir + subdir):
                shutil.copytree(subdirpath,outdir + subdir)
                print("copy folder    " + subdir)
    for subfile in os.listdir(respath):
        subfilepath = os.path.join(respath,subfile)
        if not os.path.isdir(subfilepath):
            if os.path.splitext(subfile)[1] == '.sbs' or os.path.splitext(subfile)[1] == '.sbsar':
                outfolder = os.path.splitext(subfile)[0]
                if not os.path.exists(outdir + outfolder):
                    os.makedirs(outdir + outfolder)
                    print("make dir    " + outfolder)
                if not os.path.exists(outdir + outfolder + "/" + subfile):
                    shutil.copy(subfilepath,outdir + outfolder + "/" + subfile)
                    print("    copy file    " + subfile)
    for subfile in os.listdir(respath):
        subfilepath = os.path.join(respath,subfile)
        if not os.path.isdir(subfilepath):
            if os.path.splitext(subfile)[1] == '.png' or os.path.splitext(subfile)[1] == '.jpg':
                outfolder = os.path.splitext(subfile)[0]
                if os.path.exists(outdir + outfolder):
                    if not os.path.exists(outdir + outfolder + "/thumbnail"):
                        os.makedirs(outdir + outfolder + "/thumbnail")
                        print("    make thumbnail    " + outfolder)
                    if not os.path.exists(outdir + outfolder + "/thumbnail/" + subfile):
                        shutil.copy(subfilepath,outdir + outfolder + "/thumbnail/" + subfile)
                        print("        copy png    " + subfile)
                    
def spzzzzr(respath):
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        for subfile in os.listdir(filepath):
            subfilepath = os.path.join(filepath,subfile)
            if not os.path.isdir(subfilepath):
                if os.path.splitext(subfile)[1] == '.png' or os.path.splitext(subfile)[1] == '.jpg':
                    outpath = filepath + "/thumbnail" 
                    if not os.path.exists(outpath):
                        os.makedirs(outpath)
                        print("make thumbnail in    " + file)
                    if not os.path.exists(outpath + "/" +subfile):
                        shutil.move(subfilepath,outpath + "/" +subfile)
                        print("move png to    " + outpath + "/" +subfile)
#dependencies
def formatchecker(respath):
    errorlist = []
    for folder in os.listdir(respath):
        folderpath = os.path.join(respath,folder)
        for fileordir in os.listdir(folderpath):
            fileordirfull = os.path.join(folderpath,fileordir)
            if os.path.isdir(fileordirfull):
                if fileordir != "dependencies" and fileordir != "thumbnail"and fileordir.find(".resources") == -1:
                    print("folder name error    + " + fileordirfull)
                    errorlist.append(fileordirfull)
                for root,dirs,files in os.walk(fileordirfull):
                   for file in files:  
                       filepath = os.path.join(root,file)
                       if os.path.splitext(file)[1] == '.zip' or os.path.splitext(file)[1] == '.rar' or  os.path.splitext(file)[1] == '.spsm':
                           print("contain error file     " + file)
                           errorlist.append(filepath)
            else:
                if os.path.splitext(fileordir)[1] != '.sbs' and os.path.splitext(fileordir)[1] != '.sbsar':
                    print("error format     " + fileordir)
                    errorlist.append(fileordirfull)


                    
def lackthumbcheck(respath):
    totalcount = 0
    sbsarcount = 0
    for folder in os.listdir(respath):
        folderpath = os.path.join(respath,folder)
        folderflag = False
        for fileordir in os.listdir(folderpath):
            fileordirfull = os.path.join(folderpath,fileordir)
            if os.path.isdir(fileordirfull):
                if fileordir == "thumbnail":
                    folderflag = True
        if not folderflag:
            print("lack thumbnail   +   " + folder)
            totalcount+=1
            sbsarflag= False
            for fileordir in os.listdir(folderpath):
                fileordirfull = os.path.join(folderpath,fileordir)
                if os.path.splitext(fileordir)[1] == '.sbsar':
                    sbsarflag = True
            if not sbsarflag:
                sbsarcount +=1
    return totalcount,sbsarcount

def checkformat():
    lacklist = []
    for folder in os.listdir(exrout):
        folderpath = os.path.join(exrout,folder)
        print("check file   " + folder)
        for subfolder in os.listdir(folderpath):
            subpath = os.path.join(folderpath,subfolder)
            hasThumbnail = False   
            for checkfile in os.listdir(subpath):
                checkfilepath = os.path.join(subpath,checkfile)
                if os.path.isdir(checkfilepath) and checkfile.lower().find('thumbnail') != -1:
                    hasThumbnail = True

            if not hasThumbnail:
                print("dont have thumbnail   " + subpath)
                thumbnailfolder = subpath + "/thumbnail"
                for checkfile in os.listdir(subpath):
                    checkfilepath = os.path.join(subpath,checkfile)
                    if not os.path.isdir(checkfilepath):
                        if os.path.splitext(checkfile)[1] == '.png' or os.path.splitext(checkfile)[1] == '.jpg':
                            if checkfile.lower().find('col') != -1 or checkfile.lower().find('albedo') != -1:
                                if not hasThumbnail:                               
                                    if not os.path.exists(thumbnailfolder):
                                        os.makedirs(thumbnailfolder)
                                    ResizeThumbnailImage(checkfilepath,thumbnailfolder + "/" +checkfile,512)
                                    hasThumbnail = True
                                    print("make thumbnail  " + checkfile)
                if not hasThumbnail:
                    for checkfile in os.listdir(subpath):
                        checkfilepath = os.path.join(subpath,checkfile)
                        if not os.path.isdir(checkfilepath):
                            if os.path.splitext(checkfile)[1] == '.png' or os.path.splitext(checkfile)[1] == '.jpg':
                                if not hasThumbnail:                               
                                    if not os.path.exists(thumbnailfolder):
                                        os.makedirs(thumbnailfolder)
                                    ResizeThumbnailImage(checkfilepath,thumbnailfolder + "/" +checkfile,512)
                                    hasThumbnail = True
                                    print("make thumbnail  " + checkfile)

            if not hasThumbnail:
                lacklist.append(subpath)

    for path in lacklist:
        print("lack     +    " + path)


def checkCGAxisTexture(respath,outputpath,titlename):
    for subfolder in os.listdir(respath):
        subfull = os.path.join(respath,subfolder)
        if os.path.isdir(subfull):
            outfolder = titlename + "_" + subfolder
            if subfolder.find(titlename) != -1:
                outfolder = subfolder
            outfolderpath = os.path.join(outputpath,outfolder)
            if not os.path.exists(outfolderpath):
                os.makedirs(outfolderpath)
            findthumbnai = False
            print("deal with folder   " + outfolder)
            for img in os.listdir(subfull):
                imgfull = os.path.join(subfull,img)
                if isImageformat(img):
                    if isthumbnail(img):
                        findthumbnai = True
                        outthumbfolder = os.path.join(outfolderpath,"thumbnail")
                        if not os.path.exists(outthumbfolder):
                            os.makedirs(outthumbfolder)
                        imgout = os.path.join(outthumbfolder,img)
                        ResizeImgByType(imgfull,imgout,512)
                    else:
                        imgout = os.path.join(outfolderpath,img)
                        ResizeImgByType(imgfull,imgout,2048)
            if not findthumbnai:
                print("\n\n\n\n")
                print("******** no thumbnail  *********  " + subfull)
                print("\n\n\n\n")


def ResizeImgByType(filein,fileout,maxsize):
    if os.path.splitext(filein)[1].lower() == '.jpg' or os.path.splitext(filein)[1].lower() == '.png':
        ResizeThumbnailImage(filein,fileout,maxsize)
    elif os.path.splitext(filein)[1].lower() == '.tif':
        fileout = fileout.replace('.tif','.png')
        ResizeThumbnailImageTif(filein,fileout,maxsize)
    elif os.path.splitext(filein)[1].lower() == '.exr':
        fileout = fileout.replace('.exr','.png')
        #copyfile = findUnuseName()
        #shutil.copy(filein,copyfile)
        ResizeThumbnailImageExr(filein,fileout,maxsize)

def findUnuseName():
    head = r"E:\Temp\zzr\temp"
    index = 1
    while(os.path.exists(head + str(index) + ".exr")):
        index+=1
    return head + str(index) + ".exr"

def isImageformat(filename):
    if os.path.splitext(filename)[1].lower() == '.jpg' or os.path.splitext(filename)[1].lower() == '.png':
        return True
    if os.path.splitext(filename)[1].lower() == '.tif' or os.path.splitext(filename)[1].lower() == '.exr':
        return True    
    return False


def isthumbnail(jpgtitle):
    if jpgtitle.lower().find('preview') != -1 or jpgtitle.lower().find('render') != -1:
        return True
    return False


def getcategory(foldername):
    categories = foldername.split(' ')
    category = categories[len(categories) - 1]
    return category.lower()
      
def poliigonmakercore(respath,outputpath,titlename):
    folderlist = []
    for root,dirs,files in os.walk(respath):
        for jpg in files:
            if isImageformat(jpg):
                jpgpath = os.path.join(root,jpg)
                outputfolder = titlename + jpg.split('_')[0].lower()
                jpgoutpath = outputpath + "/" + outputfolder
                if not os.path.exists(jpgoutpath) and jpgoutpath not in folderlist:
                    print("     make dir " + outputfolder)
                    os.makedirs(jpgoutpath)
                    folderlist.append(jpgoutpath)
                if jpgpath.lower().find('preview') != -1:
                    if not os.path.exists(jpgoutpath + "/thumbnail"):
                        os.makedirs(jpgoutpath + "/thumbnail")
                    ResizeImgByType(jpgpath,jpgoutpath + "/thumbnail/" + jpg,512)
                    print("        thumbnail" + jpg,flush = True)
                else:
                    ResizeImgByType(jpgpath,jpgoutpath+ "/" + jpg,2048)
                    print("     " + jpg,flush = True)

def poliigonmaker(respath):
    outpath = "E:/Temp/test/"
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        outfolder = file.lower()
        if not os.path.exists(outpath + outfolder):
            os.makedirs(outpath + outfolder)
            print("make folder " + outfolder)
        poliigonmakercore(filepath,outpath+outfolder,"poliigon_")


def poliigonmaker2(respath):
    outpath = "E:/Temp/zzr/"
    titlename = "poliigon_"
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        if not os.path.isdir(filepath) and isImageformat(file):
            if len(file.split('_')) < 3:
                outfolder =titlename + os.path.splitext(file)[0].lower().replace('_',"").replace(" ","")
                if not os.path.exists(outpath+outfolder):
                    print("make dir " + outfolder)
                    os.makedirs(outpath+outfolder)
                if not os.path.exists(outpath+outfolder + "/thumbnail"):
                    os.makedirs(outpath+outfolder + "/thumbnail")
                outputpath = outpath+outfolder + "/thumbnail/" + file
                ResizeImgByType(filepath,outputpath,512)
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        if not os.path.isdir(filepath) and isImageformat(file):
            if len(file.split('_')) >= 3:
                outfolder = titlename + file.split("_")[0].lower()
                if not os.path.exists(outpath + outfolder):
                    print("no thumbnail   "+ outfolder)
                    os.makedirs(outpath + outfolder)
                outputpath = outpath+outfolder + "/" + file
                ResizeImgByType(filepath,outputpath,2048)
                print("          img  " + outputpath)
    for file in os.listdir(respath):
       filepath = os.path.join(respath,file)
       if os.path.isdir(filepath): 
           outfolder = titlename + file.lower().replace('_',"").replace(" ","")
           if not os.path.exists(outpath + outfolder):
               print("no thumbnail   "+ outfolder)
               os.makedirs(outpath + outfolder)
           for jpg in os.listdir(filepath):
               jpgpath = os.path.join(filepath,jpg)
               if isImageformat(jpg):
                   outputpath = outpath+outfolder + "/" + jpg
                   ResizeImgByType(jpgpath,outputpath,2048)
                   print("          img  " + outputpath)


def checkthumbnaisize(jpgpath):
    maxValue = 512
    im = Image.open(jpgpath)
    if  max(im.size) > maxValue:
        coefficient = maxValue / max(im.size)
        coefficient = min(coefficient,1)
        w = math.ceil(im.size[0] * coefficient)
        h = math.ceil(im.size[1] * coefficient)
        out = im.resize((w, h),Image.ANTIALIAS)
        out.save(jpgpath,type)
        print("resize " + jpgpath)


def checkandmakethumbnail(respath):
    hasthumbnail = False
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        if os.path.isdir(filepath) and filepath.find('thumbnail') != -1:
            for jpg in os.listdir(filepath):
                if isImageformat(jpg):
                    hasthumbnail = True
                    checkthumbnaisize(os.path.join(filepath,jpg))
            break
    if not hasthumbnail:
        findcol = False
        if not os.path.exists(respath + "/thumbnail"):
            print("make thumbnail  " + respath)
            os.makedirs(respath + "/thumbnail")
        for jpg in os.listdir(respath):
            jpgpath = os.path.join(respath,jpg)
            if isImageformat(jpg):
                if jpg.lower().find('col') != -1 or jpg.lower().find('albedo') != -1 or jpg.lower().find('diff') != -1 and not findcol:
                    findcol = True
                    ResizeImgByType(jpgpath,respath + "/thumbnail/" + jpg,512)
                    print("copy and resize" + jpgpath)
                    break
        if not findcol:
            findjpg = False
            for jpg in os.listdir(respath):
                jpgpath = os.path.join(respath,jpg)
                if isImageformat(jpg) and not findjpg:
                    findjpg = True
                    ResizeImgByType(jpgpath,respath + "/thumbnail/" + jpg,512)
                    print("copy  and resize" + jpgpath)
                    break
            if not findjpg:
                print("*******Folder ERROR!!!!!!!!*********")
                print(respath)
                print("\n\n\n\n")
                shutil.rmtree(respath)

def fuckRealDisplacement(respath,outdir, titlename):
    for file in os.listdir(respath):
        filepath = os.path.join(respath,file)
        for zz in os.listdir(filepath):
            zzpath = os.path.join(filepath,zz)
            outfolder = os.path.join(outdir,titlename + zz.lower())
            if not os.path.exists(outfolder):
                os.makedirs(outfolder)
                print("makedir   " + outfolder)
            for aa in os.listdir(zzpath):
                aapath = os.path.join(zzpath,aa)
                if os.path.isdir(aapath) and aa.lower().find("demo") == -1:
                    for jpg in os.listdir(aapath):
                        jpgpath = os.path.join(aapath,jpg)
                        if isImageformat(jpg) and jpg.lower().find("8k") == -1:
                           outjpg = jpg.lower().replace("4k","2k").replace("6k","2k").replace("3k","2k")
                           outputpath = outfolder + "/" + outjpg
                           if not os.path.exists(outputpath):
                               ResizeImgByType(jpgpath,outputpath,2048)
                               print("          img  " + outputpath)
                if not os.path.isdir(aapath) and isImageformat(aa):
                    if not os.path.exists(outfolder + "/thumbnail"):
                        os.makedirs(outfolder + "/thumbnail")
                    outputpath = outfolder + "/thumbnail/" + aa
                    if not os.path.exists(outputpath):
                        ResizeImgByType(aapath,outputpath,512)
                        print("          thumbnail  " + outputpath)

def cgtradermaker(respath, outputpath, titilename):
    folderlist = []
    namemap = {}
    namelist = []
    for root,dirs,files in os.walk(respath):
        for jpg in files:
            if isImageformat(jpg):
                jpgpath = os.path.join(root,jpg)
                aa = os.path.splitext(jpg)[0].lower()
                zz = aa.split("4k")
                if len(zz) > 1:
                    name = zz[0]
                    lastname = zz[1]
                    if name not in namelist:
                        namelist.append(name)
                        namemap[name] = titilename + "ground" + str(len(namelist))
                    outputfolder = namemap[name]
                    jpgoutpath = outputpath + "/" + outputfolder
                    if not os.path.exists(jpgoutpath) and jpgoutpath not in folderlist:
                        print("make dir " + outputfolder)
                        os.makedirs(jpgoutpath)
                        folderlist.append(jpgoutpath)
                    if not os.path.exists(jpgoutpath+ "/" + namemap[name] +lastname + ".png"):
                        ResizeImgByType(jpgpath,jpgoutpath+ "/" + namemap[name] +lastname + ".png",2048)
                        print("     " +  namemap[name] +lastname + ".png",flush = True)
                else:
                    print(jpgpath)

def getdynamicsize(respath):
    imageSize = os.path.getsize(respath)
    imageSize /= 1024*1024
    if imageSize > 150:
        return 2048
    return 4096

exrin = r"D:\temoutput\Megascan2"
exrin2 = r"Z:\资源共享\美术中心资源库（请大家踊跃注入）\__Done\纹理\CGTex_100\CGTextures"
exrout = r"Z:\资源共享\美术中心资源库（请大家踊跃注入）\__Samples\Textures\_Tradition"
namelist = []
for root,dirs,files in os.walk(exrin2):
    for jpg in files:
        if isImageformat(jpg):
            cgname = os.path.splitext(jpg)[0].lower()
            if cgname not in namelist:
                namelist.append(cgname)
                zz = len(namelist)


print("name list len is   " + str(len(namelist)))

for file in os.listdir(exrout):
    filepath = os.path.join(exrout,file)
    for subfile in os.listdir(filepath):
        subfilefull = os.path.join(filepath,subfile)
        if subfile.lower() in namelist:
            outfoldername = "cgtexture_" + subfile.lower()
            os.rename(subfilefull,os.path.join(filepath,outfoldername))
            print("rename     " + os.path.join(filepath,outfoldername))
        


#fuckRealDisplacement(exrin,exrout,"realdisplacement_")
#checkthumbnaisize(exrout)
#cgtradermaker(exrin,exrout,"cgtrader_")
def meganscanmaker(respath):
    docount = 0
    allcount = 0
    for file in os.listdir(respath):
        if file.lower().find("preview") == -1:
            allcount += 1
    for file in os.listdir(respath):
        if file.lower().find("preview") == -1:
            mainfolder = exrout + "/" + file.split("_")[0].lower()
            filepath = os.path.join(respath,file)
            docount += 1
            if not os.path.exists(mainfolder):
                os.makedirs(mainfolder)
                print("make dir " + mainfolder)
            for jpg in os.listdir(filepath):
                jpgpath = os.path.join(filepath,jpg)
                if not os.path.isdir(jpgpath) and isImageformat(jpg):
                    outfolder =mainfolder + "/megascan_" + file
                    outfolder = outfolder.replace(' ','')
                    if not os.path.exists(outfolder):
                        os.makedirs(outfolder)
                        print("    make sub dir  " + outfolder  + "   %d / %d " %(docount,allcount))
                    if jpg.lower().find("preview") != -1:
                        if not os.path.exists(outfolder + "/thumbnail"):
                            os.makedirs(outfolder + "/thumbnail")
                        jpgoutpath = outfolder + "/thumbnail/" + jpg
                        if not os.path.exists(jpgoutpath.replace('.exr','.png').replace('.tif','.png')):
                            ResizeImgByType(jpgpath,jpgoutpath,512)
                            print("           thumbnail " + jpg)
                    else:
                        dynamicsize = getdynamicsize(jpgpath)
                        jpgoutpath = outfolder + "/" + jpg
                        if not os.path.exists(jpgoutpath.replace('.exr','.png').replace('.tif','.png')):
                            ResizeImgByType(jpgpath,jpgoutpath,dynamicsize)
                            print("       img size " + str(dynamicsize) + "   path " + jpg)



#meganscanmaker(exrin)
#meganscanmaker(exrin2)

    
'''
for file in os.listdir(exrout):
    filepath = os.path.join(exrout,file)
    checkandmakethumbnail(filepath)
'''




