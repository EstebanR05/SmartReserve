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

    function setResult(elementId, data) {
        const node = document.getElementById(elementId);
        if (!node) {
            return;
        }

        if (typeof data === "string") {
            node.textContent = data;
            return;
        }

        node.textContent = JSON.stringify(data, null, 2);
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
            window.location.href = "/login";
            return false;
        }

        return hasToken;
    }

    async function apiCall(url, method, body) {
        const token = getToken();
        const headers = { "Content-Type": "application/json" };
        if (token) {
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
            const fallback = { message: `Request failed (${response.status})` };
            throw new Error((data && (data.message || data.Message)) || fallback.message);
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
            await handler(new FormData(form), form);
        });
    }

    function toInt(value, defaultValue) {
        const parsed = parseInt(value, 10);
        return Number.isNaN(parsed) ? defaultValue : parsed;
    }

    function toDecimal(value, defaultValue) {
        const parsed = parseFloat(value);
        return Number.isNaN(parsed) ? defaultValue : parsed;
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
                    window.location.href = "/dashboard";
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
                await apiCall("/api/auth/register", "POST", {
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

                const loginResult = await apiCall("/api/auth/login", "POST", {
                    email: fd.get("email"),
                    password: fd.get("password")
                });

                if (loginResult && loginResult.token) {
                    saveToken(loginResult.token);
                    window.location.href = "/dashboard";
                    return;
                }

                setMessage("authResult", "Cuenta creada. Inicia sesión para continuar.", false);
            } catch (error) {
                setMessage("authResult", error.message, true);
            }
        });
    }

    function wireDashboardNavigation() {
        const navLinks = Array.from(document.querySelectorAll(".sr-nav-link"));
        const panels = Array.from(document.querySelectorAll(".sr-panel"));

        navLinks.forEach((button) => {
            button.addEventListener("click", () => {
                const panel = button.getAttribute("data-panel");
                if (!panel) {
                    return;
                }

                navLinks.forEach((n) => n.classList.remove("is-active"));
                button.classList.add("is-active");

                panels.forEach((p) => p.classList.remove("is-active"));
                const selected = document.getElementById(`panel-${panel}`);
                selected?.classList.add("is-active");
            });
        });
    }

    async function loadSites() {
        const data = await apiCall("/api/tourist-sites", "GET");
        setResult("sitesResult", data);
        return data;
    }

    async function loadUnits() {
        const data = await apiCall("/api/accommodation-units", "GET");
        setResult("unitsResult", data);
        return data;
    }

    async function loadReservations() {
        const data = await apiCall("/api/reservations/mine", "GET");
        setResult("reservationsResult", data);
        return data;
    }

    function renderOverview(stats) {
        const statsNode = document.getElementById("overviewStats");
        if (!statsNode) {
            return;
        }

        const cards = [
            { label: "Sitios", value: stats.sites },
            { label: "Unidades", value: stats.units },
            { label: "Mis Reservas", value: stats.reservations }
        ];

        statsNode.innerHTML = cards
            .map((card) => `<article class="sr-stat-card"><span>${card.label}</span><strong>${card.value}</strong></article>`)
            .join("");
    }

    async function loadOverview() {
        try {
            const [sites, units, reservations] = await Promise.all([
                loadSites(),
                loadUnits(),
                loadReservations()
            ]);

            renderOverview({
                sites: Array.isArray(sites) ? sites.length : 0,
                units: Array.isArray(units) ? units.length : 0,
                reservations: Array.isArray(reservations) ? reservations.length : 0
            });

            setResult("overviewResult", {
                sites: Array.isArray(sites) ? sites.slice(0, 2) : sites,
                units: Array.isArray(units) ? units.slice(0, 2) : units,
                reservations: Array.isArray(reservations) ? reservations.slice(0, 2) : reservations
            });
        } catch (error) {
            setResult("overviewResult", error.message);
        }
    }

    function bindDashboardActions() {
        bindSubmit("siteCreateForm", async (fd, form) => {
            try {
                const payload = {
                    name: fd.get("name"),
                    city: fd.get("city"),
                    siteType: fd.get("siteType"),
                    description: fd.get("description") || null,
                    maxCapacity: toInt(fd.get("maxCapacity"), 1),
                    isActive: fd.get("isActive") === "on"
                };

                const created = await apiCall("/api/tourist-sites", "POST", payload);
                setResult("sitesResult", { message: "Sitio creado", created });
                form.reset();
                await loadSites();
                await loadOverview();
            } catch (error) {
                setResult("sitesResult", error.message);
            }
        });

        bindSubmit("unitCreateForm", async (fd, form) => {
            try {
                const payload = {
                    touristSiteId: toInt(fd.get("touristSiteId"), 0),
                    accommodationTypeId: toInt(fd.get("accommodationTypeId"), 0),
                    code: fd.get("code"),
                    name: fd.get("name"),
                    description: fd.get("description") || null,
                    maxCapacity: toInt(fd.get("maxCapacity"), 1),
                    bedroomCount: toInt(fd.get("bedroomCount"), 0),
                    bathroomCount: toInt(fd.get("bathroomCount"), 0),
                    hasKitchen: fd.get("hasKitchen") === "on",
                    hasParking: fd.get("hasParking") === "on",
                    isActive: fd.get("isActive") === "on"
                };

                const created = await apiCall("/api/accommodation-units", "POST", payload);
                setResult("unitsResult", { message: "Unidad creada", created });
                form.reset();
                await loadUnits();
                await loadOverview();
            } catch (error) {
                setResult("unitsResult", error.message);
            }
        });

        bindSubmit("availabilitySearchForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: toInt(fd.get("touristSiteId"), 0),
                    checkInDate: fd.get("checkInDate"),
                    checkOutDate: fd.get("checkOutDate"),
                    people: fd.get("people") ? toInt(fd.get("people"), null) : null
                };

                const result = await apiCall("/api/availability/search", "POST", payload);
                setResult("availabilityResult", result);
            } catch (error) {
                setResult("availabilityResult", error.message);
            }
        });

        bindSubmit("ratesSearchForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: toInt(fd.get("touristSiteId"), 0),
                    referenceDate: fd.get("referenceDate"),
                    people: toInt(fd.get("people"), 1),
                    accommodationTypeId: toInt(fd.get("accommodationTypeId"), 0),
                    accommodationUnitId: fd.get("accommodationUnitId") ? toInt(fd.get("accommodationUnitId"), null) : null
                };

                const result = await apiCall("/api/rates/search", "POST", payload);
                setResult("ratesResult", { search: result });
            } catch (error) {
                setResult("ratesResult", error.message);
            }
        });

        bindSubmit("ratesCalculateForm", async (fd) => {
            try {
                const payload = {
                    touristSiteId: toInt(fd.get("touristSiteId"), 0),
                    referenceDate: fd.get("referenceDate"),
                    people: toInt(fd.get("people"), 1),
                    accommodationTypeId: toInt(fd.get("accommodationTypeId"), 0),
                    roomCount: toInt(fd.get("roomCount"), 1),
                    nights: toInt(fd.get("nights"), 1)
                };

                const result = await apiCall("/api/rates/calculate", "POST", payload);
                setResult("ratesResult", { calculate: result });
            } catch (error) {
                setResult("ratesResult", error.message);
            }
        });

        bindSubmit("reservationCreateForm", async (fd, form) => {
            try {
                const payload = {
                    touristSiteId: toInt(fd.get("touristSiteId"), 0),
                    checkInDate: fd.get("checkInDate"),
                    checkOutDate: fd.get("checkOutDate"),
                    adults: toInt(fd.get("adults"), 1),
                    children: toInt(fd.get("children"), 0),
                    contactFullName: fd.get("contactFullName"),
                    contactEmail: fd.get("contactEmail"),
                    contactPhone: fd.get("contactPhone") || null,
                    units: [
                        {
                            accommodationUnitId: toInt(fd.get("accommodationUnitId"), 0),
                            quantity: toInt(fd.get("quantity"), 1),
                            peopleCount: toInt(fd.get("peopleCount"), 1),
                            unitPrice: toDecimal(fd.get("unitPrice"), 0)
                        }
                    ]
                };

                const created = await apiCall("/api/reservations", "POST", payload);
                setResult("reservationsResult", { message: "Reserva creada", created });
                form.reset();
                await loadReservations();
                await loadOverview();
            } catch (error) {
                setResult("reservationsResult", error.message);
            }
        });

        bindSubmit("reservationCancelForm", async (fd, form) => {
            try {
                const reservationId = toInt(fd.get("reservationId"), 0);
                await apiCall(`/api/reservations/${reservationId}`, "DELETE");
                setResult("reservationsResult", { message: `Reserva ${reservationId} cancelada` });
                form.reset();
                await loadReservations();
                await loadOverview();
            } catch (error) {
                setResult("reservationsResult", error.message);
            }
        });

        document.getElementById("refreshSitesBtn")?.addEventListener("click", () => loadSites().catch((e) => setResult("sitesResult", e.message)));
        document.getElementById("refreshUnitsBtn")?.addEventListener("click", () => loadUnits().catch((e) => setResult("unitsResult", e.message)));
        document.getElementById("refreshReservationsBtn")?.addEventListener("click", () => loadReservations().catch((e) => setResult("reservationsResult", e.message)));
    }

    function initPortalHome() {
        const authenticated = ensureAuthenticated(true);
        if (!authenticated) {
            return;
        }

        wireDashboardNavigation();
        bindDashboardActions();
        loadOverview();

        const token = getToken();
        const userPill = document.getElementById("userPill");
        if (userPill && token) {
            userPill.textContent = "Sesión activa";
        }
    }

    window.SmartReservePortal = {
        initSignInPage,
        initRegisterPage,
        initPortalHome,
        clearToken
    };
})(window);
