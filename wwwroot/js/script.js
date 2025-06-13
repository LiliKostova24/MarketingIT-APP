// Wait until the DOM is ready before querying for elements
document.addEventListener('DOMContentLoaded', () => {
    // ======================
    // Theme-switcher helpers
    // ======================
    // (You can wire these to your own buttons later)

    window.setThemeLight = function () {
        localStorage.theme = 'light';
        document.documentElement.classList.remove('dark');
    };

    window.setThemeDark = function () {
        localStorage.theme = 'dark';
        document.documentElement.classList.add('dark');
    };

    window.setThemeAuto = function () {
        localStorage.removeItem('theme');
    };

    // ====================================
    // Image-preview handlers (safe binding)
    // ====================================

    // 1) Post creation preview
    const addPostInput = document.getElementById('addPostUrl');
    if (addPostInput) {
        addPostInput.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.readAsDataURL(this.files[0]);
                reader.addEventListener('load', function (e) {
                    const img = document.getElementById('addPostImage');
                    if (img) {
                        img.src = e.target.result;
                        img.style.display = 'block';
                    }
                });
            }
        });
    }

    // 2) Status creation preview
    const statusInput = document.getElementById('createStatusUrl');
    if (statusInput) {
        statusInput.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.readAsDataURL(this.files[0]);
                reader.addEventListener('load', function (e) {
                    const img = document.getElementById('createStatusImage');
                    if (img) {
                        img.src = e.target.result;
                        img.style.display = 'block';
                    }
                });
            }
        });
    }

    // 3) Product creation preview
    const productInput = document.getElementById('createProductUrl');
    if (productInput) {
        productInput.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.readAsDataURL(this.files[0]);
                reader.addEventListener('load', function (e) {
                    const img = document.getElementById('createProductImage');
                    if (img) {
                        img.src = e.target.result;
                        img.style.display = 'block';
                    }
                });
            }
        });
    }
});




    