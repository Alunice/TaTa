fn createServer ipAddress port = 
(
	dotNetIPAddress = (DotNetClass "System.Net.IPAddress").Parse ipAddress
	tcpListener = DotNetObject "System.Net.Sockets.TcpListener" dotNetIPAddress port
	tcpListener.Start()
	msgReceived = ""
	print ("listening on " + (ipAddress as string) + ":" + (port as string))
	while true do 
	(
		print("waiting for a client socket")
		socket = tcpListener.AcceptSocket()
		print("client socket accepted")
		sendByteStream = DotNetObject "System.Byte[]" 10000
		socket.Receive sendByteStream
		encoding = DotnetClass "System.Text.UTF8Encoding"
		msgReceived = trimright (encoding.UTF8.GetString(sendByteStream))

		if msgReceived != "" then (
			print("execute : " + msgReceived)
			try(
				result = execute(msgReceived)
			) catch (
				error_str = "***" + getCurrentException() +"***\n" 
				print(error_str)
				socket.Close()
			)
		)
		socket.Close()
	)
)

fn BackgroundTcpListenerDoWork theSender theEvent =
(
	ipAddress = "127.0.0.1"
	port = 7500
	try (
		print "execute create function!!!!!"
		createServer ipAddress port
		exit
	)
	catch(
		error_str = "***" + getCurrentException() +"***\n" 
		print(error_str)
		print("server crashed or port " + (port as string) + " is already used")
	)
)


macroScript StartTcpServer category:"UnityMeshSync"
(
	BackgroundWorker = DotNetObject "System.ComponentModel.BackgroundWorker"
	DotNet.AddEventHandler BackgroundWorker "DoWork" BackgroundTcpListenerDoWork
	BackgroundWorker.WorkerSupportsCancellation = true
	BackgroundWorker.RunWorkerAsync()
)

fn existFile fname = (getfiles fname).count != 0


macroScript ExportToUnity category:"UnityMeshSync"
(
	if existFile OutPutFilePath then
	(
		hiddenlist = #()
		for o in objects do
		(
			if o.ishidden do
			(
				append hiddenlist o
				unhide o
			)
		)

		for o in objects do selectmore o

		exportFile OutPutFilePath #noPrompt selectedOnly:true using:FBXEXP

		for o in hiddenlist do hide o
	)else (
		print "File Not Exist!!!"
	)
	

)

menuItemNum = menuMan.numMenus()
for i = 1 to menuItemNum do
(
	theItem = menuMan.getMenu(i)
	if theItem.getTitle() ==  "UnityMeshSync" do
	(
		print "catch you!!!!"
		subItemNum = theItem.numItems()
		theActionServer = menuMan.createActionItem "StartTcpServer" "UnityMeshSync" 
		theItem.addItem theActionServer(subItemNum + 1)

		theActionExport = menuMan.createActionItem "ExportToUnity" "UnityMeshSync" 
		theItem.addItem theActionExport(subItemNum + 2)

		menuMan.updateMenuBar()
		exit
	)
)
