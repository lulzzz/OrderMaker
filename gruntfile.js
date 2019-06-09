

module.exports = function (grunt) {
    'use strict';    

    const sass = require('node-sass');   
    require('load-grunt-tasks')(grunt);   
    const path = require('path');

    grunt.initConfig({       
        sass: {           
            options: {                
                implementation: sass,
                sourceMap: true,             
                outputStyle: 'compressed',
                includePaths: [
                    path.join(path.dirname(module.filename), 'node_modules')
                ]
            },
            dist: {
                files: [
                    {
                        expand: true, 
                        cwd: "wwwroot/lib/mtd-ordermaker/common/sass",
                        src: ["**/*.scss"], 
                        dest: "wwwroot/lib/mtd-ordermaker/common/css",                        
                        ext: ".css"
                    }
                ]
            }
        }
    });
    
    grunt.registerTask('default', ['sass']);
};
