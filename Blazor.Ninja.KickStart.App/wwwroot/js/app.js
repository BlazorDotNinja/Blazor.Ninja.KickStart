var ContextMenu = function () {
	this.show = function (settings) {
		var container = document.getElementById(settings.id);
		if (!container) return;

		container.style.top = settings.y + 'px';
		container.style.left = settings.x + 'px';
		container.style.display = "flex";

		window.addEventListener('keydown', function (e) {
			if (e.keyCode === 27) {

				container.style.display = "none";
				return false;
			}

			return true;
		});
	}

	this.hide = function (settings) {
		var container = document.getElementById(settings.id);
		if (!container) return;

		container.style.display = "none";
	}
}

window.contextMenu = new ContextMenu();

var BlazorNinjaElement = function () {
	this.focus = function (elementId) {
		var element = document.getElementById(elementId);
		if (!element) return;
		if (element instanceof HTMLElement) element.focus();
	}
}

window.blazorNinja = {};
window.blazorNinja.element = new BlazorNinjaElement();

window.goBack = function() {
	window.history.go(-1);
};

window.canGoBack = function() {
	return history.length > 1;
};

function downloadFile(filename, contentType, data) {
	// Convert base64 string to numbers array.
	const numberArray = atob(data).split('').map(c => c.charCodeAt(0));
	// Convert numbers array to Uint8Array object.
	const uint8Array = new Uint8Array(numberArray);
	// Wrap it by Blob object.
	const blob = new Blob([uint8Array], { type: contentType });
	// Create "object URL" that is linked to the Blob object.
	const url = URL.createObjectURL(blob);

	downloadFromUrl(url, filename);
	// At last, release unused resources.
	URL.revokeObjectURL(url);
}

function downloadFromUrl(url, fileName) {
	const anchorElement = document.createElement('a');
	anchorElement.href = url;
	anchorElement.download = fileName ?? '';
	anchorElement.click();
	anchorElement.remove();
}
