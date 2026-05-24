(function (window) {
    const TOKEN_KEY = "smartreserve.jwt";
    const state = {
        selectedSite: null,
        selectedUnits: [],
        lastAvailability: [],
        ratesByUnit: new Map()
    };

    function getToken() { return localStorage.getItem(TOKEN_KEY); }
    function saveToken(token) { localStorage.setItem(TOKEN_KEY, token); }
    function clearToken() { localStorage.removeItem(TOKEN_KEY); }

    function setMessage(elementId, message, isError) {
        const node = document.getElementById(elementId);
        if (!node) return;
        node.textContent = message;
        node.style.color = isError ? "#b42318" : "#1f3d8a";
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
        const headers = { "Content-Type": "application/json" };
        const token = getToken();
        if (token) headers.Authorization = `Bearer ${token}`;

        const response = await fetch(url, {
            method,
            headers,
            body: body ? JSON.stringify(body) : undefined
        });

        const text = await response.text();
        const data = text ? safeParseJson(text) : null;
        if (!response.ok) {
            if (response.status === 401) {
                clearToken();
                window.location.href = "/login";
            }
            throw new Error((data && (data.message || data.Message)) || `Request failed (${response.status})`);
        }
        return data;
    }

    function safeParseJson(text) {
        try { return JSON.parse(text); } catch { return { message: text }; }
    }

    function bindSubmit(formId, handler) {
        const form = document.getElementById(formId);
        if (!form) return;
        form.addEventListener("submit", async (event) => {
            event.preventDefault();
            await handler(new FormData(form), form);
        });
    }

    function number(value, fallback) {
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : fallback;
    }

    function money(value) {
        return new Intl.NumberFormat("es-CO", { style: "currency", currency: "COP", maximumFractionDigits: 0 }).format(value || 0);
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
                }
            } catch (error) {
                setMessage("authResult", error.message, true);
            }
        });
    }

    function switchView(view) {
        document.querySelectorAll(".legacy-view").forEach((el) => el.classList.remove("is-active"));
        document.querySelectorAll(".legacy-nav-link").forEach((el) => el.classList.remove("is-active"));
        document.getElementById(`view-${view}`)?.classList.add("is-active");
        document.querySelector(`.legacy-nav-link[data-view=\"${view}\"]`)?.classList.add("is-active");
    }

    function switchTab(panel) {
        document.querySelectorAll(".legacy-panel").forEach((el) => el.classList.remove("is-active"));
        document.querySelectorAll(".legacy-tab").forEach((el) => el.classList.remove("is-active"));
        document.getElementById(`panel-${panel}`)?.classList.add("is-active");
        document.querySelector(`.legacy-tab[data-panel=\"${panel}\"]`)?.classList.add("is-active");
    }

    async function loadSites() {
        const tbody = document.getElementById("sitesTableBody");
        if (!tbody) return;

        let rows = [];
        try {
            rows = await apiCall("/api/tourist-sites", "GET");
        } catch (error) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="6">
                        No se pudieron cargar las sedes: ${error.message}
                    </td>
                </tr>
            `;
            return;
        }

        if (!Array.isArray(rows) || rows.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="6">
                        No hay sedes disponibles en la base de datos.
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = (rows || []).map((site) => `
            <tr>
                <td><div style="width:120px;height:70px;background:#cfcfcf"></div></td>
                <td>${site.name || ""}</td>
                <td>${site.description || "Sin descripción"}</td>
                <td>${site.siteType || ""}</td>
                <td>${site.city || ""}</td>
                <td><button class="legacy-select-btn" data-site-id="${site.id}">Seleccionar</button></td>
            </tr>
        `).join("");

        tbody.querySelectorAll("button[data-site-id]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = Number(btn.getAttribute("data-site-id"));
                state.selectedSite = rows.find((x) => x.id === id) || null;
                state.selectedUnits = [];
                state.ratesByUnit.clear();
                document.getElementById("selectedSiteTitle").textContent = state.selectedSite?.name || "Seleccione una sede";
                document.getElementById("selectedSiteDescription").textContent = state.selectedSite?.description || "";
                switchTab("dates");
            });
        });
    }

    async function loadMyReservations() {
        const tbody = document.getElementById("myReservationsBody");
        if (!tbody) return;
        let rows = [];
        try {
            rows = await apiCall("/api/reservations/mine", "GET");
        } catch (error) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="8">No se pudieron cargar tus reservas: ${error.message}</td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = (rows || []).map((r) => `
            <tr>
                <td>${r.touristSiteName || r.touristSite?.name || r.touristSiteId}</td>
                <td>${(r.createdAtUtc || "").toString().slice(0, 10)}</td>
                <td>${r.checkInDate || ""}</td>
                <td>${r.checkOutDate || ""}</td>
                <td>${r.totalPeople ?? ((r.adults || 0) + (r.children || 0))}</td>
                <td>${(r.reservationUnits || []).length}</td>
                <td>${money(r.totalAmount || 0)}</td>
                <td><button class="legacy-select-btn" data-cancel-id="${r.id}">Cancelar</button></td>
            </tr>
        `).join("");

        tbody.querySelectorAll("button[data-cancel-id]").forEach((btn) => {
            btn.addEventListener("click", async () => {
                try {
                    await apiCall(`/api/reservations/${btn.getAttribute("data-cancel-id")}`, "DELETE");
                    await loadMyReservations();
                } catch (e) {
                    alert(e.message);
                }
            });
        });
    }

    async function searchAvailability(fd) {
        if (!state.selectedSite) {
            alert("Primero selecciona una sede.");
            return;
        }

        const payload = {
            touristSiteId: state.selectedSite.id,
            checkInDate: fd.get("checkInDate"),
            checkOutDate: fd.get("checkOutDate"),
            people: number(fd.get("people"), 1)
        };

        const availability = await apiCall("/api/availability/search", "POST", payload);
        state.lastAvailability = availability || [];

        const rates = await apiCall("/api/rates/search", "POST", {
            touristSiteId: state.selectedSite.id,
            referenceDate: fd.get("checkInDate"),
            people: number(fd.get("people"), 1),
            accommodationTypeId: 1
        });

        state.ratesByUnit.clear();
        (rates || []).forEach((r) => {
            if (r.accommodationUnitId) state.ratesByUnit.set(r.accommodationUnitId, r);
        });

        renderUnitsTable(number(fd.get("nights"), 1), number(fd.get("people"), 1));
    }

    function renderUnitsTable(nights, people) {
        const tbody = document.getElementById("unitsTableBody");
        if (!tbody) return;

        tbody.innerHTML = state.lastAvailability.map((u) => {
            const rate = state.ratesByUnit.get(u.accommodationUnitId || u.id);
            const unitId = u.accommodationUnitId || u.id;
            const name = u.accommodationUnitName || u.name || `Unidad ${unitId}`;
            const capacity = u.maxCapacity || u.capacity || "-";
            const base = rate?.basePrice || rate?.price || 0;
            const available = u.available !== false;

            return `
                <tr>
                    <td><button class="legacy-select-btn" data-detail-id="${unitId}">Detalle</button> ${name}</td>
                    <td>${capacity}</td>
                    <td>${money(base)}</td>
                    <td>${available ? "✔" : "✖"}</td>
                    <td><input type="checkbox" data-reserve-id="${unitId}" ${available ? "" : "disabled"}/></td>
                </tr>
            `;
        }).join("");

        tbody.querySelectorAll("button[data-detail-id]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = Number(btn.getAttribute("data-detail-id"));
                const row = state.lastAvailability.find((x) => (x.accommodationUnitId || x.id) === id);
                const rate = state.ratesByUnit.get(id);
                document.getElementById("unitModalBody").innerHTML = `
                    <p><strong>Habitación / Alojamiento:</strong> ${row?.accommodationUnitName || row?.name || id}</p>
                    <p><strong>Capacidad:</strong> ${row?.maxCapacity || row?.capacity || "-"}</p>
                    <p><strong>Tarifa día Ordinario:</strong> ${money(rate?.basePrice || 0)}</p>
                    <p><strong>Tarifa día Especial:</strong> ${money(rate?.additionalPersonPrice || rate?.basePrice || 0)}</p>
                `;
                openModal("unitModal");
            });
        });

        tbody.querySelectorAll("input[data-reserve-id]").forEach((cb) => {
            cb.addEventListener("change", () => {
                const id = Number(cb.getAttribute("data-reserve-id"));
                if (cb.checked) {
                    if (!state.selectedUnits.includes(id)) state.selectedUnits.push(id);
                } else {
                    state.selectedUnits = state.selectedUnits.filter((x) => x !== id);
                }
                updateSummary(nights, people);
            });
        });

        updateSummary(nights, people);
    }

    function updateSummary(nights, people) {
        const rooms = state.selectedUnits.length;
        let total = 0;
        state.selectedUnits.forEach((id) => {
            const rate = state.ratesByUnit.get(id);
            total += (rate?.basePrice || 0) * nights;
        });

        document.getElementById("summaryRooms").textContent = String(rooms);
        document.getElementById("summaryOrdinary").textContent = String(nights);
        document.getElementById("summarySpecial").textContent = "0";
        document.getElementById("summaryTotal").textContent = money(total);

        document.getElementById("openConfirmBtn").disabled = rooms === 0;
    }

    function openModal(id) {
        document.getElementById(id)?.classList.remove("hidden");
    }

    function closeModal(id) {
        document.getElementById(id)?.classList.add("hidden");
    }

    function setupConfirmReservation() {
        document.getElementById("openConfirmBtn")?.addEventListener("click", () => {
            const checkIn = document.querySelector("#availabilityForm input[name='checkInDate']")?.value;
            const checkOut = document.querySelector("#availabilityForm input[name='checkOutDate']")?.value;
            const people = number(document.querySelector("#availabilityForm input[name='people']")?.value, 1);
            const nights = number(document.querySelector("#availabilityForm input[name='nights']")?.value, 1);

            document.getElementById("confirmBody").innerHTML = `
                <p><strong>Fecha de Llegada:</strong> ${checkIn || ""}</p>
                <p><strong>Fecha de Salida:</strong> ${checkOut || ""}</p>
                <p><strong>Noches:</strong> ${nights}</p>
                <p><strong>Personas:</strong> ${people}</p>
                <p><strong>Habitaciones:</strong> ${state.selectedUnits.length}</p>
                <p><strong>Valor Total:</strong> ${document.getElementById("summaryTotal")?.textContent || "$0"}</p>
            `;
            openModal("confirmModal");
        });

        document.getElementById("confirmReservationBtn")?.addEventListener("click", async () => {
            try {
                if (!state.selectedSite || state.selectedUnits.length === 0) return;
                const checkIn = document.querySelector("#availabilityForm input[name='checkInDate']")?.value;
                const checkOut = document.querySelector("#availabilityForm input[name='checkOutDate']")?.value;
                const people = number(document.querySelector("#availabilityForm input[name='people']")?.value, 1);
                const contactFullName = document.querySelector("#availabilityForm input[name='contactFullName']")?.value || "";
                const contactEmail = document.querySelector("#availabilityForm input[name='contactEmail']")?.value || "";
                const contactPhone = document.querySelector("#availabilityForm input[name='contactPhone']")?.value || "";

                if (!contactFullName || !contactEmail) {
                    alert("Completa nombre y email de contacto para reservar.");
                    return;
                }

                const units = state.selectedUnits.map((id) => ({
                    accommodationUnitId: id,
                    quantity: 1,
                    peopleCount: people,
                    unitPrice: state.ratesByUnit.get(id)?.basePrice || 0
                }));

                await apiCall("/api/reservations", "POST", {
                    touristSiteId: state.selectedSite.id,
                    checkInDate: checkIn,
                    checkOutDate: checkOut,
                    adults: people,
                    children: 0,
                    contactFullName,
                    contactEmail,
                    contactPhone: contactPhone || null,
                    units
                });

                closeModal("confirmModal");
                state.selectedUnits = [];
                await loadMyReservations();
                switchView("my-reservations");
            } catch (e) {
                alert(e.message);
            }
        });
    }

    function initPortalHome() {
        if (!ensureAuthenticated(true)) return;

        document.querySelectorAll(".legacy-nav-link[data-view]").forEach((btn) => {
            btn.addEventListener("click", () => switchView(btn.getAttribute("data-view")));
        });
        document.querySelectorAll(".legacy-tab[data-panel]").forEach((btn) => {
            btn.addEventListener("click", () => switchTab(btn.getAttribute("data-panel")));
        });

        document.getElementById("logoutBtn")?.addEventListener("click", () => {
            clearToken();
            window.location.href = "/login";
        });

        bindSubmit("availabilityForm", async (fd) => {
            try {
                await searchAvailability(fd);
            } catch (e) {
                alert(e.message);
            }
        });

        document.querySelectorAll("[data-close]").forEach((btn) => {
            btn.addEventListener("click", () => closeModal(btn.getAttribute("data-close")));
        });

        setupConfirmReservation();
        loadSites();
        loadMyReservations();
    }

    window.SmartReservePortal = {
        initSignInPage,
        initRegisterPage,
        initPortalHome,
        clearToken
    };
})(window);
