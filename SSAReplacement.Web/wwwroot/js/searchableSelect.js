window.searchableSelect = {
  registerClickOutside: function (element, dotNetRef) {
    setTimeout(function () {
      function handler(e) {
        if (element && !element.contains(e.target)) {
          document.removeEventListener('click', handler);
          dotNetRef.invokeMethodAsync('CloseDropdown');
        }
      }
      document.addEventListener('click', handler);
    }, 0);
  }
};
