(function (window) {
    const TOKEN_KEY = "smartreserve.jwt";

    function setMessage(elementId, message, isError) {
        const node = document.getElementById(elementId);
        if (!node) {
            return;
        }

        node.textContent = message;
        node.style.color = isError ? "#b42318" : "#1f3d8a";
    }

    function getToken() {
        return localStorage.getItem(TOKEN_KEY);
    }

    function saveToken(token) {
        localStorage.setItem(TOKEN_KEY, token);
    }

    function clearToken() {
        localStorage.removeItem(TOKEN_KEY);
    }

    function ensureAuthenticated(redirectToSignIn) {
        const hasToken = !!getToken();
        if (!hasToken && redirectToSignIn) {
            window.location.href = "/Account/SignIn";
            return false;
        }

        return hasToken;
    }

    async function apiCall(url, method, body) {
        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json" },
            body: body ? JSON.stringify(body) : undefined
        });

        const text = await response.text();
        const data = text ? safeParseJson(text) : null;

        if (!response.ok) {
            const fallback = { message: `Request failed (${response.status})` };
            throw new Error((data && data.message) || fallback.message);
        }

        return data;
    }

    function safeParseJson(text) {
        try {
            return JSON.parse(text);
        } catch {
            return { message: text };
        }
    }

    function bindSubmit(formId, handler) {
        const form = document.getElementById(formId);
        if (!form) {
            return;
        }

        form.addEventListener("submit", async (event) => {
            event.preventDefault();
            await handler(new FormData(form));
        });
    }

    function initSignInPage() {
        bindSubmit("loginForm", async (fd) => {
            try {
                const result = await apiCall("/api/auth/login", "POST", {
                    email: fd.get("email"),
                    password: fd.get("password")
                });

                if (result && result.token) {
                    saveToken(result.token);
                    window.location.href = "/Portal/Index";
                    return;
                }

                setMessage("authResult", "Login completed.", false);
            } catch (error) {
                setMessage("authResult", error.message, true);
            }
        });
    }

    function initRegisterPage() {
        bindSubmit("registerForm", async (fd) => {
            try {
                const result = await apiCall("/api/auth/register", "POST", {
                    name: fd.get("name"),
                    lastName: fd.get("lastName"),
                    email: fd.get("email"),
                    password: fd.get("password"),
                    phone: fd.get("phone"),
                    address: fd.get("address"),
                    businessName: fd.get("businessName"),
                    businessAddress: fd.get("businessAddress"),
                    businessPhone: fd.get("businessPhone")
                });

                setMessage("authResult", result?.message || "Account created successfully.", false);
            } catch (error) {
                setMessage("authResult", error.message, true);
            }
        });
    }

    function initRecoveryPage() {
        bindSubmit("forgotPasswordForm", async (fd) => {
            try {
                const result = await apiCall("/api/auth/forgot-password", "POST", {
                    email: fd.get("email")
                });

                setMessage("authResult", result?.message || "Recovery email sent.", false);
            } catch (error) {
                setMessage("authResult", error.message, true);
            }
        });
    }

    function initPortalHome() {
        ensureAuthenticated(true);
    }

    window.SmartReservePortal = {
        initSignInPage,
        initRegisterPage,
        initRecoveryPage,
        initPortalHome,
        clearToken
    };
})(window);
