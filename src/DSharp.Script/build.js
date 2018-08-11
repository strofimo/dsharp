const buildify = require("buildify");
const fs = require('fs');

const concatFilesDir = "./src/System";

function getFiles(dir, files_) {
    files_ = files_ || [];
    var files = fs.readdirSync(dir);
    for (var i in files) {
        var name = dir + '/' + files[i];
        if (fs.statSync(name).isDirectory()) {
            getFiles(name, files_);
        } else {
            files_.push(name);
        }
    }
    return files_;
}

getFiles(concatFilesDir).forEach(file => {
    console.log(file);
})

buildify()
    .concat(getFiles(concatFilesDir), '\r\n')
    .wrap('src/Loader.js', {
        version: 1.0
    })
    .save('dist/ds.js')
    .uglify()
    .save('dist/ds.min.js')
