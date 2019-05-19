/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/

let gulp = require("gulp"),
    fs = require("fs"),
    less = require("gulp-less"),
    cleanCSS = require('gulp-clean-css');


gulp.task("lessFile", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/filechoose/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/filechoose/css/'));
});

gulp.task("lessConfig", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/config/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/config/css/'));
});

gulp.task("lessUsers", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/users/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/users/css/'));
});

gulp.task("lessIdentity", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/identity/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/identity/css/'));
});

gulp.task("lessDesktop", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/desktop/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/desktop/css/'));
});

gulp.task("lessMain", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/main/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/main/css/'));
});

gulp.task("lessIndex", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/index/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/index/css/'));
});

gulp.task("lessStore", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/store/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/store/css/'));
});

gulp.task("lessUiControls", function () {
    return gulp.src('wwwroot/lib/mtd-ordermaker/ui/less/mtd*.less').pipe(less()).pipe(gulp.dest('wwwroot/lib/mtd-ordermaker/ui/css/'));
});

