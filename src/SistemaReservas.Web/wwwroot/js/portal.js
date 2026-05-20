(function (window) {
    const TOKEN_KEY = "smartreserve.jwt";

    function setResult(elementId, payload) {
        const node = document.getElementById(elementId);
        if (!node) {
            return;
        }

        node.textContent = typeof payload === "string"
            ? payload
            : JSON.stringify(payload, null, 2);
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

    async function apiCall(url, method, body, requireAuth) {
        const headers = { "Content-Type": "application/json" };

        if (requireAuth) {
            const token = getToken();
            if (!token) {
                throw new Error("No JWT token found. Login first.");
            }

            headers.Authorization = `Bearer ${token}`;
        }

        const response = await fetch(url, {
            method,
            headers,
            body: body ? JSON.stringify(body) : undefined
        });

        const text = await response.text();
        const data = text ? safeParseJson(text) : null;

        if (!response.ok) {
            throw new Error(JSON.stringify(data ?? { status: response.status, message: response.statusText }, null, 2));
        }

        return data;
    }

    function safeParseJson(text) {
        try {
            return JSON.parse(text);
        } catch {
            return text;
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

    function dateValue(formData, key) {
        return (formData.get(key) || "").toString();
    }

    function intValue(formData, key, allowNull) {
        const raw = (formData.get(key) || "").toString().trim();
        if (!raw) {
            return allowNull ? null : 0;
        }

        return Number.parseInt(raw, 10);
    }

    function decimalValue(formData, key) {
        const raw = (formData.get(key) || "").toString().trim();
        return Number.parseFloat(raw);
    }

    function initSignInPage() {
        bindSubmit("loginForm", async (fd) => {
            try {
                const result = await apiCall("/api/auth/login", "POST", {
                    email: fd.get("email"),
                    password: fd.get("password")
                }, false);

                if (result && result.token) {
                    saveToken(result.token);
                    setResult("authResult", result);
                    window.location.href = "/Portal/Index";
                    return;
                }

                setResult("authResult", result);
            } catch (error) {
                setResult("authResult", error.message);
            }
        });

        bindSubmit("forgotPasswordForm", async (fd) => {
            try {
                const result = await apiCall("/api/auth/forgot-password", "POST", {
                    email: fd.get("email")
                }, false);
                setResult("authResult", result);
            } catch (error) {
                setResult("authResult", error.message);
            }
        });
    }

    function initRegisterPage() {
        bindSubmit("registerForm", async (fd) => {
            try {
                const result = await apiCall("/api/auth/register", "POST", {
                    email: fd.get("email"),
                    password: fd.get("password")
                }, false);
                setResult("authResult", result);
            } catch (error) {
                setResult("authResult", error.message);
            }
        });
    }

    function initAvailabilityPage() {
        if (!ensureAuthenticated(true)) {
            return;
        }

        bindSubmit("availabilityForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: intValue(fd, "touristSiteId"),
                    checkInDate: dateValue(fd, "checkInDate"),
                    checkOutDate: dateValue(fd, "checkOutDate"),
                    people: intValue(fd, "people", true)
                };

                const result = await apiCall("/api/availability/search", "POST", payload, true);
                setResult("availabilityResult", result);
            } catch (error) {
                setResult("availabilityResult", error.message);
            }
        });
    }

    function initRatesPage() {
        if (!ensureAuthenticated(true)) {
            return;
        }

        bindSubmit("ratesSearchForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: intValue(fd, "touristSiteId"),
                    referenceDate: dateValue(fd, "referenceDate"),
                    people: intValue(fd, "people"),
                    accommodationTypeId: intValue(fd, "accommodationTypeId"),
                    accommodationUnitId: intValue(fd, "accommodationUnitId", true)
                };

                const result = await apiCall("/api/rates/search", "POST", payload, true);
                setResult("ratesResult", result);
            } catch (error) {
                setResult("ratesResult", error.message);
            }
        });

        bindSubmit("ratesCalculateForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: intValue(fd, "touristSiteId"),
                    referenceDate: dateValue(fd, "referenceDate"),
                    people: intValue(fd, "people"),
                    accommodationTypeId: intValue(fd, "accommodationTypeId"),
                    roomCount: intValue(fd, "roomCount"),
                    nights: intValue(fd, "nights")
                };

                const result = await apiCall("/api/rates/calculate", "POST", payload, true);
                setResult("ratesResult", result);
            } catch (error) {
                setResult("ratesResult", error.message);
            }
        });
    }

    function initReservationsPage() {
        if (!ensureAuthenticated(true)) {
            return;
        }

        bindSubmit("createReservationForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: intValue(fd, "touristSiteId"),
                    checkInDate: dateValue(fd, "checkInDate"),
                    checkOutDate: dateValue(fd, "checkOutDate"),
                    adults: intValue(fd, "adults"),
                    children: intValue(fd, "children"),
                    contactFullName: (fd.get("contactFullName") || "").toString(),
                    contactEmail: (fd.get("contactEmail") || "").toString(),
                    contactPhone: (fd.get("contactPhone") || "").toString(),
                    units: [
                        {
                            accommodationUnitId: intValue(fd, "accommodationUnitId"),
                            quantity: intValue(fd, "quantity"),
                            peopleCount: intValue(fd, "peopleCount"),
                            unitPrice: decimalValue(fd, "unitPrice")
                        }
                    ]
                };

                const result = await apiCall("/api/reservations", "POST", payload, true);
                setResult("reservationResult", result);
            } catch (error) {
                setResult("reservationResult", error.message);
            }
        });

        const loadReservationsBtn = document.getElementById("loadReservationsBtn");
        if (loadReservationsBtn) {
            loadReservationsBtn.addEventListener("click", async () => {
                try {
                    const result = await apiCall("/api/reservations/mine", "GET", null, true);
                    setResult("reservationResult", result);
                } catch (error) {
                    setResult("reservationResult", error.message);
                }
            });
        }

        bindSubmit("cancelReservationForm", async (fd) => {
            try {
                const reservationId = intValue(fd, "reservationId");
                await apiCall(`/api/reservations/${reservationId}`, "DELETE", null, true);
                setResult("reservationResult", `Reservation ${reservationId} cancelled successfully.`);
            } catch (error) {
                setResult("reservationResult", error.message);
            }
        });
    }

    function initPortalHome() {
        if (!ensureAuthenticated(true)) {
            return;
        }
    }

    window.SmartReservePortal = {
        initSignInPage,
        initRegisterPage,
        initAvailabilityPage,
        initRatesPage,
        initReservationsPage,
        initPortalHome,
        clearToken
    };
})(window);
