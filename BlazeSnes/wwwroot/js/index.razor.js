const baseWidth = 256;
const baseHeight = 224;
let canvas;
let context;
let imageData;

window.initEmulatorCanvas = function(e) {
    canvas = document.getElementById('emulatorCanvas');
    context = canvas.getContext('2d');
    imageData = context.createImageData(baseWidth, baseHeight);
}
window.drawEmulatorCanvas = function(dataPtr) {
    // console.log('drawEmulatorCanvas', dataPtr);
    // convert from C# data
    imageData.data.set(Uint8ClampedArray.from(Module.HEAPU8.subarray(dataPtr, dataPtr + imageData.data.length)));
    context.putImageData(imageData, 0, 0);
    // draw
    context.drawImage(canvas, 0, 0, baseWidth, baseHeight, 0, 0, canvas.width, canvas.height);
};