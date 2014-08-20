
var azure = require('azure');
var blobService = azure.createBlobService('valeryjacobs', 'w5kNlXuwyWbeQ/L3TGguPMViL/LCRU3i2kt8/Q9TD/6/2KEhrEXFFu5tqmjFWSkHBfnN/XYR5QoUT2b8ujcWcw==');
var each = require('each');
var http = require('http'),
    util = require('util'),
    formidable = require('formidable'),
    fs = require('fs'),
    server;
var mysocket;

blobService.createContainerIfNotExists('node-demo-container'
    , { publicAccessLevel: 'blob' }
    , function (error) {
        if (!error) {
            // Container exists and is public
        }
    });

    server = http.createServer(function (req, res) {
        if (req.url == '/') 
		{
            fs.readFile('index.html',
			function (err, data) 
			{
				  if (err) 
				  {
					  res.writeHead(500);
					  return res.end('Error loading index.html');
				  }

				  res.writeHead(200);
				  res.end(data);
			});
        } 
        else if (req.url == '/upload') 
		{
            var form = new formidable.IncomingForm(),
            files = [],
            fields = [],
            keepExtensions = false;

            form.uploadDir = '/temp';

            form.on('field', function (field, value) 
			{
				// console.log(field, value);
				fields.push([field, value]);
            }).on('file', function (field, file) 
			{
			  // console.log(field, file);
			  files.push([field, file]);
			}).on('end', function () 	
			{
				// console.log('-> upload done');
				res.writeHead(200, { 'content-type': 'text/plain' });
				//res.write('received fields:\n\n '+util.inspect(fields));
				res.write('\n\n');
				//res.end('received files:\n\n '+util.inspect(files));

				each(files).on('item', function (element, index, next) 
				{
					//mysocket.broadcast("uploading file to azure");
					console.log('element: ', element.valueOf('path'));

					blobService.createBlockBlobFromFile('node-demo-container'
						 , element[1].name
						 , element[1].path
						 , function (error) {
							 if (!error) {
								 console.log("File uploaded to Azure.")
								 // File has been uploaded
							 }
						 });

					setTimeout(next, 500);
				}).on('both', function (err) 
				{
					if (err) {
						console.log(err.message);
					} else {
						console.log('Done');
					}
				});
		  });
        form.parse(req);
        } else {
            res.writeHead(404, { 'content-type': 'text/plain' });
            res.end('404');
        }
    });

var io = require('socket.io').listen(server);

server.listen(8183);

io.sockets.on('connection', function (socket) {
    mysocket = socket;

    mysocket.emit('news', { hello: 'world' });
    mysocket.on('my other event', function (data) {
        console.log(data);
    });
});
console.log('listening on http://localhost:' + 8183 + '/');


