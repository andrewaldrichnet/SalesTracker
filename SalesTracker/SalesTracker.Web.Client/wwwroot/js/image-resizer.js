// Image resizer utility for Blazor WebAssembly
window.imageResizer = {
    resizeImage: function (dataUri, maxWidth, maxHeight) {
        return new Promise((resolve) => {
            const img = new Image();
            img.onload = function () {
                // Calculate new dimensions maintaining aspect ratio
                let { newWidth, newHeight } = calculateDimensions(
                    img.width,
                    img.height,
                    maxWidth,
                    maxHeight
                );

                // Create canvas and resize
                const canvas = document.createElement('canvas');
                canvas.width = newWidth;
                canvas.height = newHeight;

                const ctx = canvas.getContext('2d');
                ctx.drawImage(img, 0, 0, newWidth, newHeight);

                // Return as base64 data URI
                const resizedDataUri = canvas.toDataURL('image/jpeg', 0.85); // 85% quality to compress
                resolve(resizedDataUri);
            };
            img.onerror = function () {
                // If image fails to load, return original
                resolve(dataUri);
            };
            img.src = dataUri;
        });
    }
};

function calculateDimensions(originalWidth, originalHeight, maxWidth, maxHeight) {
    // If image is smaller than max, don't resize
    if (originalWidth <= maxWidth && originalHeight <= maxHeight) {
        return { newWidth: originalWidth, newHeight: originalHeight };
    }

    // Calculate aspect ratio
    const aspectRatio = originalWidth / originalHeight;

    // Calculate new dimensions
    if (originalWidth > originalHeight) {
        // Landscape orientation
        const newWidth = Math.min(originalWidth, maxWidth);
        const newHeight = Math.round(newWidth / aspectRatio);
        return { newWidth, newHeight };
    } else {
        // Portrait orientation
        const newHeight = Math.min(originalHeight, maxHeight);
        const newWidth = Math.round(newHeight * aspectRatio);
        return { newWidth, newHeight };
    }
}
