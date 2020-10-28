# Thumbnail Render Script

写在最前面：参考 https://zhuanlan.zhihu.com/p/80361232 文章写的批量生成缩略图的脚本

最近在做资源库整理的工作，很多资源只有sbs或者sbsar文件，缺少缩略图，不能直观的看见资源所呈现出来的效果。在按照一定规则处理完资源存储格式之后，给没有缩略图的资源制作缩略图也就提上了议程。

看了很多帖子，包括参考链接所列的文章，Substance Designer在19年的更新中，新增了PBR Render Node,缩略图的渲染也就是基于该结点进行的，文章里步骤写得很详细了，这里就只记录debug过程中遇到的坑。

## 踩过的坑，流过的泪


* **节点属性获取方式**
参考链接使用[官方文档示例](https://docs.substance3d.com/sddoc/nodes-and-properties-172825056.html)获取节点属性，相关代码如下：
```
# Get and print information regarding the selected nodes.
def printSelectedNodesInfo(nodes):
    for node in nodes:
        definition = node.getDefinition()
        nodeId = node.getIdentifier()

        print("node %s, id = %s" % (definition.getLabel(), nodeId))

        # Create a list of each property category enumeration item.
        categories = [
            SDPropertyCategory.Annotation,
            SDPropertyCategory.Input,
            SDPropertyCategory.Output
        ]

        # Get node properties for each property category.
        for category in categories:
            props = definition.getProperties(category)
```
因为本人也是刚开始学习SD，接下来的分析可能有不对的地方~
使用definition好像只能获取节点暴露出来的接口，也就是在sbstance player中打开，可以编辑的那些参数。我调试了几个sbsar文件，只有SDPropertyCategory.Input这个filter下获取到了若干properties，与player中打开看到的参数一致。
但我希望获取到的是SDPropertyCategory.Output下的诸如basecolor,normal等参数，用改方法得到的是一个空的SDARRAY。
经过文档查阅，发现SDNode节点本身也有getProperties这个方法，将代码改为：
```
inprops = materialNode.getProperties(SDPropertyCategory.Output)
	for prop in inprops:
		propId = prop.getId()
		label = prop.getLabel()
```
得到了想要的结果。


* **官方api手册一言难尽的阅读体验** 
查阅api其实是个非常基本的操作，最开始是因为想要能够控制缩略图大小，在PBR Render Node面版中看到了Output Size属性，调试时候打印出来的变量名称为"$outputsize"，类型为"SDTypeInt2"，接下来就是血坑的初始化方式了。
在官方文档中，只给了这么一段：
```
static sNew()
	Create new SDTypeInt2
	Return type SDTypeInt2
```
说实话，文档里既没有提如何修改SDTypeInt2类型的值，也没有提初始化的方法，就只是列一个接口名称，一口老血就想喷出来。
刚开始猜测是不是用两个int来初始化，然后console报错SDTypeInt2.sNew只接受一个参数的初始化。嗯，使用方式全靠猜，当你把错误答案都填过一遍，正确答案就能找到了。
我尝试了各种vector变量，都报错，无奈换了一种方式去文档查询，这回搜索的是"$outputsize"，查到了这么一段：
```
Label: ‘Value Processor’
Description: ‘The <b>Value Processor</b> filter evaluates a function for a single value’
SDPropertyCategory = ‘Input’

	‘$outputsize’ = SDValueInt2(int2(8,8))
		Label: ‘Output Size’
		Description: ‘The Output Size parameter defines the horizontal and vertical size of the output image as a power of 2. Parameter can be inherited from parent object, or set to an absolute value’
		Types:‘int2’ (SDTypeInt2)
```
然后再去查SDTypeInt2和‘int2’的使用方式，终于找到了SDTypeInt2的初始化方式：
```
from sd.api.sdvalueint2 import SDValueInt2
from sd.api.sdbasetypes import int2
instanceNode.setInputPropertyValueFromId('$outputsize', SDValueInt2.sNew(int2(-1,-1)))
```
只能说，官方真是个数据结构设计鬼才，小机灵蛋一枚呢。不过好处就是，踩过这两个坑之后，再查阅官方文档就顺利许多了，没有太多的问题。（虽然还有很多接口的调用方式都是靠猜，莫名其妙对了那就对了吧）

* **SD脚本运行crash** 
这个也是挺无语的一件事，看论坛中的讨论是说，SD刚上PY那会儿，根本没法用，现在好歹能跑起来了，在处理一些比较大的资源的时候，你只能静静地看着console打印log，如果不小心点了鼠标或是啥，SD很容易直接未响应，莫名其妙。
这个顺带的问题就是，官方手册里推荐了VSC作为debug工具，但我调试了一下午，也没办法remote到SD上面debug，就更别提打断点什么的了，程序跑起来想要中断，似乎也就只有任务管理器大法了orz

暂时就记录这些吧，刚刚开始摸索，社区啥的也不怎么样，一点点慢慢学⑧
