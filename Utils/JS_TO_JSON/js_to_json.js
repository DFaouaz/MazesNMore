
const fs = require('fs') 

function from_js_to_json(input_dir, filename, output_dir) {
    var objectString = fs.readFileSync(input_dir + filename, 'utf-8');

    var jsonStr = objectString.replace(/(\w+:)|(\w+ :)/g, function(s) {
        return '"' + s.substring(0, s.length-1) + '":';
      });

    var object = JSON.parse(jsonStr);
    objectString = JSON.stringify(object);

    fs.writeFileSync(output_dir + filename, objectString);

}


var files = fs.readdirSync('.\\input\\', 'utf-8');

for(let i = 0; i < files.length; i++) 
  from_js_to_json('.\\input\\', files[i], ".\\output\\");
