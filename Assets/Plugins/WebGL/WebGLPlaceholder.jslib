mergeInto(LibraryManager.library, {
    WebGLInputForceBlur: function () {
        console.log("WebGLInputForceBlur called (placeholder)");
    },
    WebGLInputInit: function () {
        console.log("WebGLInputInit called (placeholder)");
    },
    WebGLWindowGetCanvasName: function () {
        console.log("WebGLWindowGetCanvasName called (placeholder)");
        var str = "unity-canvas";
        var buffer = allocate(intArrayFromString(str), 'i8', ALLOC_NORMAL);
        return buffer;
    },
    WebGLWindowInit: function () {
        console.log("WebGLWindowInit called (placeholder)");
    },
    WebGLWindowOnResize: function () {
        console.log("WebGLWindowOnResize called (placeholder)");
    }
});
