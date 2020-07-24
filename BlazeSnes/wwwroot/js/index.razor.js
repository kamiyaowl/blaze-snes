window.drawEmulatorCanvas = function(dataPtr) {
    // console.log('drawEmulatorCanvas', dataPtr);
    var canvas = document.getElementById('emulatorCanvas');
    var context = canvas.getContext('2d');
    var imageData = context.createImageData(256, 224);

    imageData.data.set(Uint8ClampedArray.from(Module.HEAPU8.subarray(dataPtr, dataPtr + imageData.data.length)));
    context.putImageData(imageData, 0, 0);
    context.drawImage(canvas, 0, 0, canvas.width, canvas.height);
};