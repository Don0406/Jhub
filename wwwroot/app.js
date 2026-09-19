/**
 * J-Hub Core Application Logic
 * Handles RBAC Routing and Global UI interactions
 */

document.addEventListener("DOMContentLoaded", () => {
    
    // ==========================================
    // 1. AUTHENTICATION & ROUTING
    // ==========================================
    const loginForm = document.getElementById("login-form");

    if (loginForm) {
        loginForm.addEventListener("submit", (e) => {
            e.preventDefault();
            
            const submitBtn = loginForm.querySelector('button[type="submit"]');
            const emailInput = document.getElementById("email").value.toLowerCase();
            const passwordInput = document.getElementById("password").value.toLowerCase();
            
            // 1. Lock the button and show loading state
            submitBtn.innerText = "Authenticating...";
            submitBtn.disabled = true;
            submitBtn.style.opacity = "0.7";

            // 2. THE ERROR SIMULATION (If password is "wrong" or "fail")
            if (passwordInput === "wrong" || passwordInput === "fail") {
                setTimeout(() => {
                    // Show error toast
                    showToast('Invalid credentials. Please check your email and password.', 'error');
                    
                    // Reset the button so they can try again
                    submitBtn.innerText = "Log In";
                    submitBtn.disabled = false;
                    submitBtn.style.opacity = "1";
                    
                    // Clear the password field
                    document.getElementById("password").value = "";
                }, 800);
                return; // Stop the login process here!
            }

            // 3. THE SUCCESS ROUTE
            let userRole = determineRole(emailInput);
            showToast(`Verifying ${userRole} credentials...`, 'info');
            
            setTimeout(() => {
                showToast('Access Granted. Initiating secure session.', 'success');
                setTimeout(() => {
                    routeUser(userRole);
                }, 800);
            }, 1200);
        });
    }

    // ==========================================
    // PAGE-SPECIFIC INITIALIZERS
    // ==========================================
    initGlobalLogout();

    if (window.location.pathname.includes("agency-catalog.html")) {
        initAgencyCatalog();
    }
    if (window.location.pathname.includes("agency-dashboard.html")) {
        initAgencyDashboard();
    }
    if (window.location.pathname.includes("booking-confirmation.html")) {
        initBookingConfirmation();
    }
    if (window.location.pathname.includes("booking-queue.html")) {
        initBookingQueue();
    }
    if (window.location.pathname.includes("joiner-dashboard.html")) {
        initJoinerDashboard();
    }
    if (window.location.pathname.includes("booking-status-detail.html")) {
        console.log("Router detected the status page!");
        initBookingStatusDetail();
    }
    if (window.location.pathname.includes("client-management.html")) {
        initClientManagement();
    }
    if (window.location.pathname.includes("commission-tracker.html")) {
        initCommissionTracker();
    }
    if (window.location.pathname.includes("create-tour.html")) {
        initCreateTour();
    }
    if (window.location.pathname.includes("financial-dashboard.html")) {
        initFinancialDashboard();
    }
    if (window.location.pathname.includes("flash-sale-modal.html")) {
        initThresholdModal();
    }
    if (window.location.pathname.includes("forgot-password.html")) {
        initForgotPassword();
    }
    if (window.location.pathname.includes("landing.html") || window.location.pathname === "/" || window.location.pathname.endsWith("index.html")) {
        initLandingPage();
    }
    if (window.location.pathname.includes("notification.html") || window.location.pathname.includes("notifications.html")) {
        initNotifications();
    }
    if (window.location.pathname.includes("mobile-")) {
        initMobileApp();
    }
    if (window.location.pathname.includes("offline-fallback.html")) {
        showToast("Network Disconnected. Displaying cached data.", "error");
    }
    if (window.location.pathname.includes("operator-dashboard.html")) {
        initOperatorDashboard();
    }
    if (window.location.pathname.includes("payment-upload.html")) {
        initPaymentUpload();
    }
    if (window.location.pathname.includes("register.html")) {
        initRegister();
    }
    if (window.location.pathname.includes("reports-exports.html")) {
        initReports();
    }
    if (window.location.pathname.includes("session-timeout.html")) {
        initSessionTimeout();
    }
    if (window.location.pathname.includes("system-settings.html")) {
        initSystemSettings();
    }
    if (window.location.pathname.includes("tour-detail.html")) {
        initTourDetails();
    }
    if (window.location.pathname.includes("tour-discovery.html")) {
        initTourDiscovery();
        initTourDiscoveryImages();
    }
    if (window.location.pathname.includes("tour-instance-details.html")) {
        initTourInstanceDetails();
    }
    if (window.location.pathname.includes("tour-management.html")) {
        initTourManagement();
    }
    if (window.location.pathname.includes("user-management.html")) {
        initUserManagement();
    }
    if (window.location.pathname.includes("vendor-management.html")) {
        initVendorManagement();
    }
    if (window.location.pathname.includes("booking-form.html")) {
        initBookingForm();
    }
    if (window.location.pathname.includes("track-booking.html")) {
        initTrackBooking();
    }
    if (window.location.pathname.includes("booking-confirmation.html")) {
        initBookingConfirmation();
    }
    initTourCreation();
});

function determineRole(email) {
    if (email.includes("operator") || email.includes("admin")) return "operator";
    if (email.includes("agency") || email.includes("reseller")) return "agency";
    if (email.includes("staff") || email.includes("field")) return "field_staff";
    return "joiner"; 
}

function routeUser(role) {
    switch(role) {
        case "operator": window.location.href = "operator-dashboard.html"; break;
        case "agency": window.location.href = "agency-dashboard.html"; break;
        case "field_staff": window.location.href = "mobile-login.html"; break;
        case "joiner": window.location.href = "joiner-dashboard.html"; break;
        default: window.location.href = "404.html";
    }
}

// ==========================================
// 2. GLOBAL UI UTILITIES
// ==========================================

function openTab(evt, tabName) {
    const tabcontent = document.getElementsByClassName("tab-content");
    for (let i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
        tabcontent[i].classList.remove("active");
    }

    const tablinks = document.getElementsByClassName("tab-btn");
    for (let i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    document.getElementById(tabName).style.display = "block";
    document.getElementById(tabName).classList.add("active");
    
    if(evt) evt.currentTarget.className += " active";
}

function showToast(message, type = 'info') {
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = message;

    container.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'fadeOut 0.3s ease forwards';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// ==========================================
// 3. MODULE: AGENCY CATALOG
// ==========================================
function initAgencyCatalog() {
    const searchInput = document.querySelector('input[placeholder="Search destinations..."]');
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            const tableRows = document.querySelectorAll('.data-table tbody tr');

            tableRows.forEach(row => {
                const rowText = row.innerText.toLowerCase();
                if (rowText.includes(searchTerm)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });
    }

    const bookButtons = document.querySelectorAll('.data-table .btn-primary');
    bookButtons.forEach(button => {
        button.addEventListener('click', (e) => {
            const row = e.target.closest('tr');
            if(!row) return; 
            const tourName = row.querySelector('.heading-md') ? row.querySelector('.heading-md').innerText : 'Tour';
            
            showToast(`Initiating client booking for <b>${tourName}</b>...`, 'success');
            
            const originalText = button.innerText;
            button.innerText = "Processing...";
            button.style.opacity = "0.7";
            button.disabled = true;

            setTimeout(() => {
                button.innerText = originalText;
                button.style.opacity = "1";
                button.disabled = false;
            }, 2000);
        });
    });
}

// ==========================================
// 4. MODULE: AGENCY DASHBOARD
// ==========================================
function initAgencyDashboard() {
    const summaryCards = document.querySelectorAll('.summary-card');
    
    summaryCards.forEach(card => {
        card.style.cursor = 'pointer';
        
        card.addEventListener('click', (e) => {
            const labelEl = card.querySelector('.label-text');
            if(!labelEl) return;
            const cardTitle = labelEl.innerText;
            
            if (cardTitle === 'PENDING COMMISSIONS' || cardTitle === 'EARNED THIS MONTH') {
                showToast('Loading Financial Ledger...', 'info');
                setTimeout(() => window.location.href = 'commission-tracker.html', 1000);
            } else if (cardTitle === 'TOTAL CLIENT BOOKINGS') {
                showToast('Opening Client Database...', 'info');
                setTimeout(() => window.location.href = 'client-management.html', 1000);
            }
        });
    });

    const tourCards = document.querySelectorAll('.tour-card');
    tourCards.forEach(card => {
        card.addEventListener('click', (e) => {
            const headingEl = card.querySelector('.heading-md');
            if(!headingEl) return;
            const tourName = headingEl.innerText;
            showToast(`Fetching live inventory and manifest for <b>${tourName}</b>...`, 'info');
            
            setTimeout(() => {
                window.location.href = 'agency-catalog.html'; 
            }, 1500);
        });
    });
}

// ==========================================
// 5. MODULE: BOOKING CONFIRMATION
// ==========================================
function initBookingConfirmation() {
    setTimeout(() => {
        showToast('Secure payload transmitted. Awaiting operator validation ledger entry.', 'success');
    }, 300);
}

// ==========================================
// 6. MODULE: BOOKING QUEUE
// ==========================================
function initBookingQueue() {
    const tableBody = document.querySelector('.data-table tbody');
    const pendingCountText = document.querySelector('.main-content > div .body-text');

    function updateCounter() {
        const rowCount = document.querySelectorAll('.data-table tbody tr').length;
        if (pendingCountText) {
            pendingCountText.innerText = `${rowCount} Pending Validations`;
        }
    }

    if (tableBody) {
        tableBody.addEventListener('click', (e) => {
            const btn = e.target;
            
            if (btn.tagName !== 'BUTTON') return;

            const row = btn.closest('tr');
            const joinerName = row.querySelector('.heading-md').innerText;

            if (btn.innerText.includes('View PoP')) {
                showToast(`Loading Proof of Payment for ${joinerName}...`, 'info');
                setTimeout(() => window.location.href = 'booking-status-detail.html', 1000);
            }
            
            else if (btn.innerText.includes('Approve')) {
                showToast(`Booking approved for ${joinerName}. Manifest updated.`, 'success');
                row.style.transition = 'opacity 0.3s, transform 0.3s';
                row.style.opacity = '0';
                row.style.transform = 'translateX(20px)';
                
                setTimeout(() => {
                    row.remove();
                    updateCounter();
                }, 300);
            }
            
            else if (btn.innerText.includes('Reject')) {
                let isConfirmed = confirm(`Are you sure you want to reject ${joinerName}'s booking?`);
                if (isConfirmed) { 
                    showToast(`Booking rejected. Resubmission request sent to ${joinerName}.`, 'error');
                    row.style.transition = 'opacity 0.3s, transform 0.3s';
                    row.style.opacity = '0';
                    row.style.transform = 'translateX(-20px)';
                    
                    setTimeout(() => {
                        row.remove();
                        updateCounter();
                    }, 300);
                }
            }
        });
    }
    
    updateCounter();
}

// ==========================================
// 7. MODULE: JOINER DASHBOARD
// ==========================================
function initJoinerDashboard() {
    const tableBody = document.querySelector('#joiner-bookings-table tbody');
    
    if (tableBody) {
        tableBody.addEventListener('click', (e) => {
            const row = e.target.closest('tr');
            if (!row) return;

            const tourName = row.querySelector('.heading-md').innerText;
            showToast(`Loading booking status for <b>${tourName}</b>...`, 'info');

            setTimeout(() => {
                window.location.href = 'booking-status-detail.html';
            }, 1000);
        });
    }

    const bellIcon = document.getElementById('notification-bell');
    if (bellIcon) {
        bellIcon.addEventListener('click', () => {
            showToast('You have 1 pending validation update.', 'info');
        });
    }
}

// ==========================================
// 8. MODULE: BOOKING STATUS DETAIL
// ==========================================
function initBookingStatusDetail() {
    setTimeout(() => {
        showToast('Syncing live validation status with operator...', 'info');
    }, 400);

    const paymentPanels = document.querySelectorAll('.panel');
    if (paymentPanels.length > 0) {
        const paymentBox = paymentPanels[0].querySelector('div[style*="background-color: var(--bg-primary)"]');
        if (paymentBox) {
            paymentBox.style.cursor = 'pointer';
            paymentBox.title = "Click to view receipt";
            paymentBox.addEventListener('click', () => {
                showToast('Opening secure Proof of Payment document...', 'success');
            });
        }
    }
}

// ==========================================
// 9. MODULE: CLIENT MANAGEMENT
// ==========================================
function initClientManagement() {
    const tableBody = document.querySelector('.data-table tbody');

    if (tableBody) {
        tableBody.addEventListener('click', (e) => {
            const btn = e.target;
            if (btn.tagName !== 'BUTTON') return;

            const row = btn.closest('tr');
            const clientName = row.querySelector('.heading-md').innerText;

            if (btn.innerText.includes('View Details')) {
                showToast(`Loading client dossier and itinerary for ${clientName}...`, 'info');
            } 
            else if (btn.innerText.includes('Upload PoP')) {
                // 1. Show opening portal toast
                showToast(`Opening secure upload portal for ${clientName}...`, 'info');
                
                // 2. Simulate upload process
                const originalText = btn.innerText;
                btn.innerText = "Uploading...";
                btn.disabled = true;
                btn.style.opacity = "0.7";

                setTimeout(() => {
                    // Change button to secondary
                    btn.innerText = "View Details";
                    btn.className = "btn btn-secondary";
                    btn.disabled = false;
                    btn.style.opacity = "1";
                    
                    // Update the badge in the row
                    const badge = row.querySelector('.badge');
                    if(badge) {
                        badge.className = 'badge';
                        badge.innerText = 'Validation Pending';
                        badge.style.backgroundColor = 'rgba(234, 179, 8, 0.2)';
                        badge.style.color = 'var(--status-watch)';
                    }

                    showToast(`Proof of Payment submitted. Operator validation pending.`, 'success');
                }, 2000);
            }
        });
    }
}

// ==========================================
// 10. MODULE: COMMISSION TRACKER
// ==========================================
function initCommissionTracker() {
    // 1. Make the Download Button interactive
    const downloadBtn = document.querySelector('.main-content .btn-secondary');
    if (downloadBtn) {
        downloadBtn.addEventListener('click', () => {
            const originalText = downloadBtn.innerText;
            downloadBtn.innerText = "Compiling CSV...";
            downloadBtn.disabled = true;
            downloadBtn.style.opacity = "0.7";

            setTimeout(() => {
                showToast('Ledger successfully downloaded to your device.', 'success');
                downloadBtn.innerText = originalText;
                downloadBtn.disabled = false;
                downloadBtn.style.opacity = "1";
            }, 1500);
        });
    }
}

// ==========================================
// 11. MODULE: CREATE TOUR
// ==========================================
function initCreateTour() {
    const form = document.getElementById('create-tour-form');
    const cancelBtn = document.getElementById('cancel-btn');

    if (form) {
        form.addEventListener('submit', (e) => {
            e.preventDefault(); // Stop the page from instantly refreshing
            
            const submitBtn = form.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerText;
            
            // Visual loading state
            submitBtn.innerText = "Saving to Database...";
            submitBtn.disabled = true;
            submitBtn.style.opacity = "0.7";

            showToast('Validating tour parameters...', 'info');

            // Simulate the backend save delay
            setTimeout(() => {
                showToast('Tour successfully created!', 'success');
                
                // Redirect back to the management table
                setTimeout(() => {
                    window.location.href = 'tour-management.html';
                }, 1000);
            }, 1500);
        });
    }

    if (cancelBtn) {
        cancelBtn.addEventListener('click', () => {
            window.location.href = 'tour-management.html';
        });
    }
}

// ==========================================
// 12. MODULE: FINANCIAL DASHBOARD
// ==========================================
function initFinancialDashboard() {
    // 1. Interactive Export Button
    const exportBtn = document.querySelector('.main-content .btn-secondary');
    if (exportBtn && exportBtn.innerText.includes('Export')) {
        exportBtn.addEventListener('click', () => {
            const originalText = exportBtn.innerText;
            exportBtn.innerText = "Generating Report...";
            exportBtn.disabled = true;
            exportBtn.style.opacity = "0.7";

            setTimeout(() => {
                showToast('Fiduciary Ledger exported securely to CSV.', 'success');
                exportBtn.innerText = originalText;
                exportBtn.disabled = false;
                exportBtn.style.opacity = "1";
            }, 1500);
        });
    }

    // 2. Live Table Filtering via Dropdowns
    const selects = document.querySelectorAll('.main-content select');
    if (selects.length === 2) {
        const tourFilter = selects[0];
        const stateFilter = selects[1];
        const tableRows = document.querySelectorAll('.data-table tbody tr');

        function applyFilters() {
            const tourVal = tourFilter.value;
            const stateVal = stateFilter.value;

            tableRows.forEach(row => {
                const rowText = row.innerText;
                const matchTour = tourVal === 'All Tours' || rowText.includes(tourVal);
                const matchState = stateVal === 'All States' || rowText.includes(stateVal);
                
                if (matchTour && matchState) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        }

        tourFilter.addEventListener('change', () => {
            showToast(`Filtering ledger by ${tourFilter.value}...`, 'info');
            applyFilters();
        });
        
        stateFilter.addEventListener('change', () => {
            showToast(`Filtering ledger by ${stateFilter.value} state...`, 'info');
            applyFilters();
        });
    }
}

// ==========================================
// 13. MODULE: THRESHOLD INTERVENTIONS (MODAL)
// ==========================================
function initThresholdModal() {
    const closeBtn = document.getElementById('close-modal-btn');
    const flashSaleBtn = document.getElementById('launch-flash-sale-btn');
    const mergeBtn = document.getElementById('confirm-merge-btn');

    // 1. Close Modal
    if (closeBtn) {
        closeBtn.addEventListener('click', () => {
            window.history.back(); // Or window.location.href = 'tour-management.html';
        });
    }

    // 2. Launch Flash Sale
    if (flashSaleBtn) {
        flashSaleBtn.addEventListener('click', () => {
            const originalText = flashSaleBtn.innerText;
            flashSaleBtn.innerText = "Broadcasting...";
            flashSaleBtn.disabled = true;
            flashSaleBtn.style.opacity = "0.7";

            setTimeout(() => {
                showToast('Flash Sale activated! Agencies notified of new discounted rate.', 'success');
                setTimeout(() => {
                    // Route back to tour management after success
                    window.location.href = 'tour-management.html';
                }, 1500);
            }, 1000);
        });
    }

    // 3. Confirm Merge
    if (mergeBtn) {
        mergeBtn.addEventListener('click', () => {
            const originalText = mergeBtn.innerText;
            mergeBtn.innerText = "Merging Manifests...";
            mergeBtn.disabled = true;
            mergeBtn.style.opacity = "0.7";

            setTimeout(() => {
                showToast('Tours successfully merged. Threshold updated to GO status.', 'success');
                setTimeout(() => {
                    // Route back to tour management after success
                    window.location.href = 'tour-management.html';
                }, 1500);
            }, 1000);
        });
    }
}

// ==========================================
// 14. MODULE: FORGOT PASSWORD
// ==========================================
function initForgotPassword() {
    const form = document.getElementById('forgot-password-form');
    
    if (form) {
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const submitBtn = form.querySelector('button[type="submit"]');
            const emailInput = form.querySelector('input[type="email"]').value;
            const originalText = submitBtn.innerText;
            
            // Visual loading state
            submitBtn.innerText = "Transmitting...";
            submitBtn.disabled = true;
            submitBtn.style.opacity = "0.7";

            // Simulate the backend SMTP delay
            setTimeout(() => {
                showToast(`Secure reset link sent to ${emailInput}`, 'success');
                
                // Route back to login after showing success
                setTimeout(() => {
                    window.location.href = 'login.html';
                }, 2000);
            }, 1200);
        });
    }
}

// ==========================================
// 15. MODULE: LANDING PAGE
// ==========================================
function initLandingPage() {
    // Fire a welcoming system-status toast shortly after the page loads
    setTimeout(() => {
        showToast('System Online. Welcome to J-Hub.', 'success');
    }, 800);
}

// ==========================================
// 16. MODULE: GLOBAL LOGOUT
// ==========================================
function initGlobalLogout() {
    // Finds any button with the ID 'logout-btn' or class 'logout-btn'
    const logoutBtns = document.querySelectorAll('#logout-btn, .logout-btn');
    
    logoutBtns.forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.preventDefault();
            
            showToast('Ending secure session...', 'info');
            
            // Visually disable the button so they don't double-click
            btn.style.opacity = '0.5';
            btn.style.pointerEvents = 'none';

            // Simulate the backend session destruction
            setTimeout(() => {
                window.location.href = 'login.html';
            }, 1000);
        });
    });
}

// ==========================================
// 17. MODULE: NOTIFICATIONS INBOX
// ==========================================
function initNotifications() {
    const markReadBtn = document.getElementById('mark-read-btn');
    
    if (markReadBtn) {
        markReadBtn.addEventListener('click', () => {
            const unreadCards = document.querySelectorAll('.notification-card.unread');
            
            if (unreadCards.length > 0) {
                // Remove the unread class from all cards
                unreadCards.forEach(card => {
                    card.classList.remove('unread');
                });
                
                showToast('All notifications marked as read.', 'success');
                
                // Dim the button since there's nothing left to read
                markReadBtn.style.opacity = '0.5';
                markReadBtn.style.pointerEvents = 'none';
            } else {
                showToast('No unread notifications.', 'info');
            }
        });
    }
}

// ==========================================
// 18. MODULE: MOBILE FIELD OPERATIONS
// ==========================================
function initMobileApp() {
    // 1. Mobile Login Form (Now with Error Simulation!)
    const mobileLoginForm = document.getElementById('mobile-login-form');
    if (mobileLoginForm) {
        mobileLoginForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const btn = mobileLoginForm.querySelector('button[type="submit"]');
            const passwordInput = mobileLoginForm.querySelector('input[type="password"]');
            const passwordValue = passwordInput.value.toLowerCase();

            btn.innerText = "Authenticating...";
            btn.disabled = true;
            btn.style.opacity = "0.7";

            // THE ERROR SIMULATION
            if (passwordValue === "wrong" || passwordValue === "fail") {
                setTimeout(() => {
                    showToast('Invalid credentials. Please check your staff ID and password.', 'error');
                    
                    // Reset button and clear password
                    btn.innerText = "Secure Log In";
                    btn.disabled = false;
                    btn.style.opacity = "1";
                    passwordInput.value = ""; 
                }, 800);
                return; // Stop the login process here!
            }

            // THE SUCCESS ROUTE
            setTimeout(() => {
                btn.innerText = "Syncing Manifests...";
                setTimeout(() => {
                    window.location.href = 'mobile-manifest.html';
                }, 800);
            }, 600);
        });
    }

    // 2. Interactive Manifest Check-ins
    const checkinToggles = document.querySelectorAll('.checkin-toggle');
    checkinToggles.forEach(toggle => {
        toggle.addEventListener('change', (e) => {
            if (e.target.checked) {
                showToast('Passenger checked in. Syncing to central server...', 'success');
            } else {
                showToast('Check-in reversed.', 'info');
            }
        });
    });

    // 3. Flagged Passenger Routing
    const flaggedPassenger = document.getElementById('flagged-passenger');
    if (flaggedPassenger) {
        flaggedPassenger.addEventListener('click', () => {
            window.location.href = 'mobile-compliance-modal.html';
        });
    }

    // 4. Modal Dismissal
    const dismissBtn = document.getElementById('dismiss-modal-btn');
    if (dismissBtn) {
        dismissBtn.addEventListener('click', () => {
            window.location.href = 'mobile-manifest.html';
        });
    }
}

// ==========================================
// 19. MODULE: OPERATOR COMMAND CENTER
// ==========================================
function initOperatorDashboard() {
    // 1. Export Financials Button
    const exportBtn = document.getElementById('export-fin-btn');
    if (exportBtn) {
        exportBtn.addEventListener('click', () => {
            const originalText = exportBtn.innerText;
            exportBtn.innerText = "Compiling Report...";
            exportBtn.disabled = true;
            exportBtn.style.opacity = "0.7";

            setTimeout(() => {
                showToast('Financial Summary successfully exported to CSV.', 'success');
                exportBtn.innerText = originalText;
                exportBtn.disabled = false;
                exportBtn.style.opacity = "1";
            }, 1200);
        });
    }

    // 2. Open Queue Button
    const queueBtn = document.getElementById('open-queue-btn');
    if (queueBtn) {
        queueBtn.addEventListener('click', () => {
            window.location.href = 'booking-queue.html';
        });
    }

    // 3. Table "View" Buttons
    const tableBody = document.querySelector('#operator-tours-table tbody');
    if (tableBody) {
        tableBody.addEventListener('click', (e) => {
            if (e.target.tagName === 'BUTTON') {
                showToast('Loading Tour Manifest...', 'info');
                setTimeout(() => {
                    window.location.href = 'tour-instance-details.html';
                }, 800);
            }
        });
    }

    // 4. Interactive Sidebar Alerts
    const urgentAlert = document.getElementById('urgent-alert');
    if (urgentAlert) {
        urgentAlert.addEventListener('click', () => {
            // Clicking the urgent alert opens the flash sale modal!
            window.location.href = 'flash-sale-modal.html';
        });
    }

    const infoAlert = document.getElementById('info-alert');
    if (infoAlert) {
        infoAlert.addEventListener('click', () => {
            // Clicking the info alert takes you to the queue!
            window.location.href = 'booking-queue.html';
        });
    }
}

// ==========================================
// 20. MODULE: PAYMENT UPLOAD
// ==========================================
function initPaymentUpload() {
    // Retrieve the saved data from the previous pages
    const savedTotal = localStorage.getItem('checkoutTotal');
    const savedTourName = localStorage.getItem('bookedTourName'); 

    const amountDisplay = document.getElementById('final-amount-due');
    const tourNameDisplay = document.getElementById('payment-tour-name'); 
    
    // Inject the dynamic price
    if (savedTotal && amountDisplay) {
        amountDisplay.innerText = savedTotal;
    }

    // Inject the dynamic tour name
    if (savedTourName && tourNameDisplay) {
        tourNameDisplay.innerText = savedTourName;
    }

    // Proof of Payment Elements
    const popUpload = document.getElementById('pop-upload');
    const popDisplay = document.getElementById('file-name-display');
    const popDropArea = document.getElementById('file-drop-area');

    // Government ID Elements (Added to fix the bug)
    const idUpload = document.getElementById('id-upload');
    const idDisplay = document.getElementById('id-name-display');
    const idDropArea = document.getElementById('id-drop-area');

    const submitBtn = document.getElementById('submit-payment-btn');
    const cancelBtn = document.getElementById('cancel-payment-btn');

    // 1. Handle Proof of Payment Selection
    if (popUpload && popDisplay) {
        popUpload.addEventListener('change', function() {
            if (this.files && this.files.length > 0) {
                popDisplay.innerText = this.files[0].name;
                popDisplay.style.color = "var(--status-go)";
                popDropArea.style.borderColor = "var(--status-go)";
                showToast('Receipt attached.', 'success');
            }
        });
    }

    // 2. Handle Government ID Selection (Added to fix the bug)
    if (idUpload && idDisplay) {
        idUpload.addEventListener('change', function() {
            if (this.files && this.files.length > 0) {
                idDisplay.innerText = this.files[0].name; // Safely targets the ID box
                idDisplay.style.color = "var(--status-go)";
                idDropArea.style.borderColor = "var(--status-go)";
                showToast('Government ID attached.', 'success');
            }
        });
    }

    // 3. Handle Submission
    if (submitBtn) {
        submitBtn.addEventListener('click', () => {
            // Check if BOTH files are uploaded
            const hasPop = popUpload && popUpload.files.length > 0;
            const hasId = idUpload && idUpload.files.length > 0;

            if (!hasPop || !hasId) {
                showToast('Please upload both your receipt and Government ID.', 'error');
                
                // Highlight the specific missing boxes in red
                if (!hasPop && popDropArea) {
                    popDropArea.style.borderColor = 'var(--status-nogo)';
                    setTimeout(() => popDropArea.style.borderColor = 'var(--border-color)', 2000);
                }
                if (!hasId && idDropArea) {
                    idDropArea.style.borderColor = 'var(--status-nogo)';
                    setTimeout(() => idDropArea.style.borderColor = 'var(--border-color)', 2000);
                }
                return;
            }

            // Success Simulation
            submitBtn.innerText = "Processing Payment...";
            submitBtn.disabled = true;
            submitBtn.style.opacity = "0.7";

            setTimeout(() => {
                showToast('Payment verified! Finalizing booking...', 'success');
                setTimeout(() => {
                    window.location.href = 'booking-confirmation.html';
                }, 1000);
            }, 1500);
        });
    }

    // 4. Handle Cancel
    if (cancelBtn) {
        cancelBtn.addEventListener('click', () => {
            window.history.back();
        });
    }
}

// ==========================================
// 21. MODULE: JOINER REGISTRATION
// ==========================================
function initRegister() {
    const registerForm = document.getElementById('register-form');
    
    if (registerForm) {
        registerForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const submitBtn = registerForm.querySelector('button[type="submit"]');
            const nameInput = document.getElementById('full-name').value;
            
            // Extract just the first name for a friendlier greeting
            const firstName = nameInput.split(' ')[0] || "Traveler";
            
            // Visual loading state
            submitBtn.innerText = "Creating Account...";
            submitBtn.disabled = true;
            submitBtn.style.opacity = "0.7";

            showToast('Encrypting and saving credentials...', 'info');

            // Simulate the backend database creation delay
            setTimeout(() => {
                showToast(`Registration successful! Welcome to J-Hub, ${firstName}.`, 'success');
                
                // Route directly to the tour discovery page so they can start booking
                setTimeout(() => {
                    window.location.href = 'tour-discovery.html';
                }, 1500);
            }, 1200);
        });
    }
}

// ==========================================
// 22. MODULE: REPORTS & EXPORTS
// ==========================================
function initReports() {
    // Select all our specific export buttons
    const exportBtns = [
        document.getElementById('export-pdf-btn'),
        document.getElementById('export-csv-btn'),
        document.getElementById('export-bir-btn'),
        document.getElementById('export-occ-btn')
    ];

    exportBtns.forEach(btn => {
        if (btn) {
            btn.addEventListener('click', () => {
                const originalText = btn.innerText;
                
                // Determine the format for the success message
                let format = "Data";
                if (originalText === "PDF" || originalText === "CSV") {
                    format = originalText;
                }

                // Show loading state
                btn.innerText = "Compiling...";
                btn.disabled = true;
                btn.style.opacity = "0.7";

                // Simulate the backend query and file generation
                setTimeout(() => {
                    showToast(`${format} Report successfully compiled and downloaded.`, 'success');
                    
                    // Reset button
                    btn.innerText = originalText;
                    btn.disabled = false;
                    btn.style.opacity = "1";
                }, 1500);
            });
        }
    });
}

// ==========================================
// 23. MODULE: SESSION TIMEOUT
// ==========================================
function initSessionTimeout() {
    // Auto-fire a warning toast when the page loads
    setTimeout(() => {
        showToast('Your secure session has timed out due to inactivity.', 'error');
    }, 500);

    const reloginBtn = document.getElementById('relogin-btn');
    if (reloginBtn) {
        reloginBtn.addEventListener('click', () => {
            // Visual loading state
            reloginBtn.innerText = "Routing to Login...";
            reloginBtn.disabled = true;
            reloginBtn.style.opacity = "0.7";
            
            setTimeout(() => {
                window.location.href = 'login.html';
            }, 600);
        });
    }
}

// ==========================================
// 24. MODULE: SYSTEM SETTINGS
// ==========================================
function initSystemSettings() {
    const settingsForm = document.getElementById('settings-form');
    
    if (settingsForm) {
        settingsForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const submitBtn = settingsForm.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerText;
            
            // Visual loading state
            submitBtn.innerText = "Applying Changes...";
            submitBtn.disabled = true;
            submitBtn.style.opacity = "0.7";

            // Simulate backend save
            setTimeout(() => {
                showToast('Global configurations updated successfully.', 'success');
                
                // Reset button
                submitBtn.innerText = originalText;
                submitBtn.disabled = false;
                submitBtn.style.opacity = "1";
            }, 1200);
        });
    }
}

// ==========================================
// 25. MODULE: DYNAMIC TOUR DETAILS & MATH
// ==========================================
function initTourDetails() {
    // 1. The Mock Database (Now with Images!)
    const toursDB = {
        "sagada": {
            title: "Sagada Weekend Retreat",
            seatsText: "Only 3 seats remaining!",
            maxSeats: 3, 
            price: 3500,
            image: "assets/Sagada-44.jpg", // <-- ADDED
            itinerary: [
                "Kiltepan Peak Sunrise Viewing",
                "Sumaguing Cave Spelunking",
                "Echo Valley Hanging Coffins",
                "Sagada Weaving & Pottery"
            ]
        },
        "pulag": {
            title: "Mt. Pulag Sea of Clouds",
            seatsText: "8 seats remaining!",
            maxSeats: 8, 
            price: 2800,
            image: "assets/pulag.jpg", // <-- ADDED
            itinerary: [
                "Ranger Station Briefing",
                "Camp 2 Setup & Stargazing",
                "Summit Sunrise Trek",
                "Ambuklao Dam Sidetrip"
            ]
        },
        "batanes": {
            title: "Batanes Heritage Escape",
            seatsText: "Only 5 seats remaining!",
            maxSeats: 5, 
            price: 12500,
            image: "assets/Batanes-1-1600x1066.jpg", // <-- ADDED
            itinerary: [
                "North Batan Tour (Basco Lighthouse)",
                "South Batan Tour (Marlboro Hills)",
                "Sabtang Island Tour",
                "Honesty Coffee Shop"
            ]
        },
        "siargao": {
            title: "Siargao Island Surf Camp",
            seatsText: "10 seats remaining!",
            maxSeats: 10, 
            price: 6000,
            image: "assets/siargao.jpg", // <-- ADDED
            itinerary: [
                "Cloud 9 Surfing Lessons",
                "Sugba Lagoon Paddleboarding",
                "Magpupungko Rock Pools",
                "Island Hopping (Naked, Daku, Guyam)"
            ]
        }
    };

    // 2. Read the URL to figure out which tour was clicked
    const urlParams = new URLSearchParams(window.location.search);
    let tourId = urlParams.get('id');

    // Default to sagada if someone loads the page without clicking a link
    if (!tourId || !toursDB[tourId]) {
        tourId = "sagada"; 
    }

    const currentTour = toursDB[tourId];

    // 3. Inject data into the HTML dynamically
    const titleEl = document.getElementById('dynamic-tour-title');
    if (titleEl) titleEl.innerText = currentTour.title;
    
    const seatsEl = document.getElementById('dynamic-tour-seats');
    if (seatsEl) seatsEl.innerText = currentTour.seatsText;
    
    const basePriceEl = document.getElementById('dynamic-base-price');
    if (basePriceEl) basePriceEl.innerText = `₱${currentTour.price.toLocaleString()}`;
    
    // Inject itinerary list
    const itineraryUl = document.getElementById('dynamic-tour-itinerary');
    if (itineraryUl) {
        itineraryUl.innerHTML = ""; // Clear the "loading" text
        currentTour.itinerary.forEach(item => {
            const li = document.createElement('li');
            li.innerText = item;
            itineraryUl.appendChild(li);
        });
    }

    // --- NEW: Inject Hero Image ---
    const heroImgContainer = document.querySelector('.hero-image');
    if (heroImgContainer && currentTour.image) {
        heroImgContainer.style.backgroundImage = `url('${currentTour.image}')`;
        heroImgContainer.style.backgroundSize = "cover";
        heroImgContainer.style.backgroundPosition = "center";
        heroImgContainer.innerHTML = ""; // Clears out the "[Destination Image Placeholder]" text
    }
    // ------------------------------

    // 4. Setup Math Variables based on the dynamic price
    const basePrice = currentTour.price;
    const maxAllowedSeats = currentTour.maxSeats; // Pull the limit from the DB
    const seatCountInput = document.getElementById('seat-count');
    const totalPriceSpan = document.getElementById('total-price');
    const confirmBtn = document.getElementById('confirm-btn');
    const waiverCheck = document.getElementById('waiver-check');

    // Set initial total price on the button
    if (totalPriceSpan) totalPriceSpan.innerText = `₱${basePrice.toLocaleString()}`;

    // Calculate dynamic price on seat change AND enforce the limit
    if (seatCountInput && totalPriceSpan) {
        
        // Dynamically update the HTML max attribute just in case
        seatCountInput.setAttribute('max', maxAllowedSeats);

        seatCountInput.addEventListener('input', (e) => {
            let seats = parseInt(e.target.value);

            // THE CLAMP LOGIC: Prevent them from typing a higher number
            if (seats > maxAllowedSeats) {
                seats = maxAllowedSeats;
                e.target.value = maxAllowedSeats;
                showToast(`Only ${maxAllowedSeats} seats remaining for this tour!`, 'error');
            } else if (seats < 1 || isNaN(seats)) {
                seats = 1;
                e.target.value = 1;
            }

            let total = seats * basePrice;
            totalPriceSpan.innerText = `₱${total.toLocaleString()}`;
        });
    }

    // Handle Waiver and Submit
    if (waiverCheck && confirmBtn) {
        waiverCheck.addEventListener('change', function() {
            confirmBtn.disabled = !this.checked;
        });

        confirmBtn.addEventListener('click', () => {
            // Your existing saving logic
            const finalSeats = seatCountInput ? parseInt(seatCountInput.value) : 1;
            localStorage.setItem('checkoutSeats', finalSeats);
            localStorage.setItem('checkoutTotal', totalPriceSpan.innerText);
            localStorage.setItem('bookedTourName', currentTour.title);

            // --- THE KILL SWITCH: TURN OFF AGENCY MODE ---
            localStorage.removeItem('isAgencyBooking');
            // ---------------------------------------------

            showToast('Seats secured. Redirecting to passenger manifest...', 'info');
            confirmBtn.innerText = "Processing...";
            confirmBtn.disabled = true;

            setTimeout(() => {
                window.location.href = 'booking-form.html';
            }, 800);
        });
    }
}

// ==========================================
// 26. MODULE: TOUR DISCOVERY (CATALOG)
// ==========================================
function initTourDiscovery() {
    localStorage.removeItem('isAgencyBooking');
    const searchInput = document.getElementById('search-input');
    const filterBtn = document.getElementById('apply-filter-btn');
    const tourCards = document.querySelectorAll('.tour-card');
    const detailBtns = document.querySelectorAll('.detail-btn');

    // 1. Live Search Filtering
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            
            tourCards.forEach(card => {
                // Read the custom data-title attribute we added in HTML
                const title = card.getAttribute('data-title');
                
                if (title.includes(searchTerm)) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    }

    // 2. Filter Button Simulation
    if (filterBtn) {
        filterBtn.addEventListener('click', () => {
            const originalText = filterBtn.innerText;
            filterBtn.innerText = "Searching...";
            
            setTimeout(() => {
                showToast('Displaying available tours based on filters.', 'info');
                filterBtn.innerText = originalText;
            }, 600);
        });
    }

    // 3. Route to Details Page
    detailBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            btn.innerText = "Loading...";
            setTimeout(() => {
                window.location.href = 'tour-detail.html';
            }, 400);
        });
    });
}

// ==========================================
// 27. MODULE: TOUR INSTANCE DETAILS
// ==========================================
function initTourInstanceDetails() {
    // 1. Tab Switching Logic
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    tabBtns.forEach(btn => {
        btn.addEventListener('click', (e) => {
            // Remove active class from all buttons and contents
            tabBtns.forEach(b => b.classList.remove('active'));
            tabContents.forEach(c => {
                c.style.display = 'none';
                c.classList.remove('active');
            });

            // Add active class to clicked button
            const targetId = btn.getAttribute('data-target');
            btn.classList.add('active');
            
            // Show corresponding content
            const targetContent = document.getElementById(targetId);
            if(targetContent) {
                targetContent.style.display = 'block';
                targetContent.classList.add('active');
            }
        });
    });

    // 2. Action Buttons in Manifest
    const mergeBtn = document.getElementById('prescriptive-merge-btn');
    if (mergeBtn) {
        mergeBtn.addEventListener('click', () => {
            showToast('Loading algorithm recommendations...', 'info');
            setTimeout(() => {
                window.location.href = 'flash-sale-modal.html';
            }, 600);
        });
    }

    const validatePopBtn = document.getElementById('validate-pop-btn');
    if (validatePopBtn) {
        validatePopBtn.addEventListener('click', () => {
            // Route the operator to the queue to actually look at Maria's receipt
            window.location.href = 'booking-queue.html';
        });
    }
}

// ==========================================
// 28. MODULE: TOUR MANAGEMENT MASTER LIST
// ==========================================
function initTourManagement() {
    const createBtn = document.getElementById('create-tour-btn');
    const searchInput = document.getElementById('search-tours-input');
    const statusFilter = document.getElementById('status-filter');
    const tableRows = document.querySelectorAll('#tour-management-table tbody tr');

    // 1. Route to Create Tour
    if (createBtn) {
        createBtn.addEventListener('click', () => {
            window.location.href = 'create-tour.html';
        });
    }

    // 2. Live Table Filtering
    function filterTable() {
        if (!searchInput || !statusFilter) return;
        
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value;

        tableRows.forEach(row => {
            const text = row.innerText.toLowerCase();
            const matchesSearch = text.includes(searchTerm);
            const matchesStatus = statusTerm === 'All Statuses' || row.innerText.includes(statusTerm);

            if (matchesSearch && matchesStatus) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }

    if (searchInput) searchInput.addEventListener('input', filterTable);
    if (statusFilter) statusFilter.addEventListener('change', filterTable);

    // 3. Edit & Archive Actions
    const tableBody = document.querySelector('#tour-management-table tbody');
    if (tableBody) {
        tableBody.addEventListener('click', (e) => {
            
            // Edit Button clicked
            if (e.target.classList.contains('edit-btn')) {
                showToast('Loading tour configuration...', 'info');
                setTimeout(() => {
                    window.location.href = 'create-tour.html';
                }, 500);
            }
            
            // Archive Button clicked
            if (e.target.classList.contains('archive-btn')) {
                const btn = e.target;
                const row = btn.closest('tr');
                const tourName = row.querySelector('.heading-md').innerText;
                
                // Confirm before archiving
                if(confirm(`Are you sure you want to archive ${tourName}? This will remove it from public view.`)) {
                    btn.innerText = "Archiving...";
                    btn.disabled = true;
                    row.style.opacity = "0.5";
                    
                    setTimeout(() => {
                        showToast(`${tourName} has been securely archived.`, 'success');
                        row.remove(); // Actually remove it from the table!
                    }, 1000);
                }
            }
        });
    }
}

// ==========================================
// 29. MODULE: USER MANAGEMENT (RBAC)
// ==========================================
function initUserManagement() {
    const userTable = document.getElementById('user-table');
    const editModal = document.getElementById('edit-user-modal');
    const closeEditBtn = document.getElementById('close-edit-modal-btn');
    const editForm = document.getElementById('edit-user-form');
    let activeEditRow = null;

    // 1 & 2. EVENT DELEGATION (Handles clicks on both old AND newly added rows)
    if (userTable) {
        userTable.addEventListener('click', (e) => {
            
            // DELETE USER LOGIC
            if (e.target.classList.contains('delete-btn')) {
                const row = e.target.closest('tr');
                const userName = row.querySelector('td').innerText; 
                if(confirm(`Are you sure you want to permanently delete user: ${userName}? This action cannot be undone.`)) {
                    row.remove();
                    showToast(`User ${userName} deleted successfully.`, "success");
                }
            }
            
            // OPEN EDIT MODAL LOGIC
            if (e.target.classList.contains('edit-user-btn')) {
                activeEditRow = e.target.closest('tr');
                const cols = activeEditRow.querySelectorAll('td');
                
                const name = cols[0].innerText;
                const role = cols[2].innerText.trim();
                const status = cols[3].innerText.trim();

                document.getElementById('edit-user-name-display').innerText = name;
                
                // Pre-fill Dropdowns
                Array.from(document.getElementById('edit-user-role').options).forEach(opt => opt.selected = (opt.value === role));
                Array.from(document.getElementById('edit-user-status').options).forEach(opt => opt.selected = (opt.value === status));

                editModal.style.display = 'flex';
            }
        });
    }

    // 3. SAVE EDITS FROM MODAL
    if (editForm) {
        editForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const newRole = document.getElementById('edit-user-role').value;
            const newStatus = document.getElementById('edit-user-status').value;

            // Update Role visually
            let badgeBg = newRole === 'Field Staff' ? '#334155' : (newRole === 'Operator' ? 'rgba(59, 130, 246, 0.2)' : 'rgba(168, 85, 247, 0.2)');
            let badgeColor = newRole === 'Field Staff' ? 'white' : (newRole === 'Operator' ? 'var(--accent-blue)' : '#A855F7');
            activeEditRow.querySelectorAll('td')[2].innerHTML = `<span class="badge" style="background-color: ${badgeBg}; color: ${badgeColor};">${newRole}</span>`;

            // Update Status visually
            let statusColor = newStatus === 'Active' ? 'var(--status-go)' : 'var(--status-nogo)';
            activeEditRow.querySelectorAll('td')[3].innerHTML = `<span class="status-text" style="color: ${statusColor}; font-weight: 600;">${newStatus}</span>`;

            editModal.style.display = 'none';
            showToast("User profile updated successfully.", "success");
        });
    }

    if (closeEditBtn) closeEditBtn.addEventListener('click', () => editModal.style.display = 'none');


    // 4. ADD NEW USER (Actually creates and appends the row)
    const addModal = document.getElementById('add-user-modal');
    const openAddBtn = document.getElementById('open-modal-btn');
    const closeAddBtn = document.getElementById('close-modal-btn');
    const addForm = document.getElementById('add-user-form');

    if (openAddBtn) openAddBtn.addEventListener('click', () => addModal.style.display = 'flex');
    if (closeAddBtn) closeAddBtn.addEventListener('click', () => addModal.style.display = 'none');

    if (addForm) {
        addForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            // Grab the input data
            const name = document.getElementById('new-user-name').value;
            const email = document.getElementById('new-user-email').value;
            const role = document.getElementById('new-user-role').value;
            const status = document.getElementById('new-user-status').value;

            // Generate exact styling variables
            let badgeBg = role === 'Field Staff' ? '#334155' : (role === 'Operator' ? 'rgba(59, 130, 246, 0.2)' : 'rgba(168, 85, 247, 0.2)');
            let badgeColor = role === 'Field Staff' ? 'white' : (role === 'Operator' ? 'var(--accent-blue)' : '#A855F7');
            let statusColor = status === 'Active' ? 'var(--status-go)' : 'var(--status-nogo)';

            // Build the new row!
            const tbody = document.querySelector('#user-table tbody');
            const newRow = document.createElement('tr');
            newRow.innerHTML = `
                <td class="heading-md">${name}</td>
                <td class="body-text">${email}</td>
                <td><span class="badge" style="background-color: ${badgeBg}; color: ${badgeColor};">${role}</span></td>
                <td><span class="status-text" style="color: ${statusColor}; font-weight: 600;">${status}</span></td>
                <td style="display: flex; gap: 8px;">
                    <button class="btn btn-secondary edit-user-btn" style="padding: 6px 12px; font-size: 12px;">Edit</button>
                    <button class="btn delete-btn" style="padding: 6px 12px; font-size: 12px; background: transparent; border: 1px solid var(--status-nogo); color: var(--status-nogo);">Delete</button>
                </td>
            `;
            
            // Push it to the table
            tbody.appendChild(newRow);

            showToast(`${name} successfully added to the system.`, "success");
            addModal.style.display = 'none';
            addForm.reset(); // Clears the form for the next time
        });
    }
}

// ==========================================
// 30. MODULE: VENDOR MANAGEMENT
// ==========================================
function initVendorManagement() {
    const vendorTable = document.getElementById('vendor-table');
    const editModal = document.getElementById('edit-vendor-modal');
    const closeEditBtn = document.getElementById('close-edit-vendor-btn');
    const editForm = document.getElementById('edit-vendor-form');
    let activeEditRow = null;

    // 1 & 2. EVENT DELEGATION (Handles Delete and Edit clicks on all rows)
    if (vendorTable) {
        vendorTable.addEventListener('click', (e) => {
            
            // DELETE VENDOR LOGIC
            if (e.target.classList.contains('delete-btn')) {
                const row = e.target.closest('tr');
                const vendorName = row.querySelector('td').innerText; 
                if(confirm(`Are you sure you want to permanently remove vendor: ${vendorName}?`)) {
                    row.remove();
                    showToast(`Vendor ${vendorName} deleted successfully.`, "success");
                }
            }
            
            // OPEN EDIT MODAL LOGIC
            if (e.target.classList.contains('edit-vendor-btn')) {
                activeEditRow = e.target.closest('tr');
                const cols = activeEditRow.querySelectorAll('td');
                
                const name = cols[0].innerText;
                const type = cols[1].innerText.trim();
                const status = cols[4].innerText.trim();

                document.getElementById('edit-vendor-name-display').innerText = name;
                
                // Pre-fill Dropdowns
                Array.from(document.getElementById('edit-vendor-type').options).forEach(opt => opt.selected = (opt.value === type));
                Array.from(document.getElementById('edit-vendor-status').options).forEach(opt => opt.selected = (opt.value === status));

                editModal.style.display = 'flex';
            }
        });
    }

    // 3. SAVE EDITS FROM MODAL
    if (editForm) {
        editForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const newType = document.getElementById('edit-vendor-type').value;
            const newStatus = document.getElementById('edit-vendor-status').value;

            // Update Type visually
            let badgeBg = newType === 'Transport' ? 'rgba(59, 130, 246, 0.2)' : (newType === 'Accommodation' ? 'rgba(168, 85, 247, 0.2)' : 'rgba(34, 197, 94, 0.2)');
            let badgeColor = newType === 'Transport' ? 'var(--accent-blue)' : (newType === 'Accommodation' ? '#A855F7' : 'var(--status-go)');
            activeEditRow.querySelectorAll('td')[1].innerHTML = `<span class="badge" style="background-color: ${badgeBg}; color: ${badgeColor};">${newType}</span>`;

            // Update Status visually
            let statusColor = newStatus === 'Active' ? 'var(--status-go)' : 'var(--status-watch)';
            activeEditRow.querySelectorAll('td')[4].innerHTML = `<span class="status-text" style="color: ${statusColor}; font-weight: 600;">${newStatus}</span>`;

            editModal.style.display = 'none';
            showToast("Vendor record updated successfully.", "success");
        });
    }

    if (closeEditBtn) closeEditBtn.addEventListener('click', () => editModal.style.display = 'none');


    // 4. ADD NEW VENDOR
    const addModal = document.getElementById('add-vendor-modal');
    const openAddBtn = document.getElementById('open-vendor-modal-btn');
    const closeAddBtn = document.getElementById('close-vendor-modal-btn');
    const addForm = document.getElementById('add-vendor-form');

    if (openAddBtn) openAddBtn.addEventListener('click', () => addModal.style.display = 'flex');
    if (closeAddBtn) closeAddBtn.addEventListener('click', () => addModal.style.display = 'none');

    if (addForm) {
        addForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            // Grab inputs
            const name = document.getElementById('new-vendor-name').value;
            const type = document.getElementById('new-vendor-type').value;
            const contact = document.getElementById('new-vendor-contact').value;
            const terms = document.getElementById('new-vendor-terms').value;
            const status = document.getElementById('new-vendor-status').value;

            // Generate exact styling variables
            let badgeBg = type === 'Transport' ? 'rgba(59, 130, 246, 0.2)' : (type === 'Accommodation' ? 'rgba(168, 85, 247, 0.2)' : 'rgba(34, 197, 94, 0.2)');
            let badgeColor = type === 'Transport' ? 'var(--accent-blue)' : (type === 'Accommodation' ? '#A855F7' : 'var(--status-go)');
            let statusColor = status === 'Active' ? 'var(--status-go)' : 'var(--status-watch)';

            // Build the new row
            const tbody = document.querySelector('#vendor-table tbody');
            const newRow = document.createElement('tr');
            newRow.innerHTML = `
                <td class="heading-md">${name}</td>
                <td><span class="badge" style="background-color: ${badgeBg}; color: ${badgeColor};">${type}</span></td>
                <td class="body-text">${contact}</td>
                <td class="body-text">${terms}</td>
                <td><span class="status-text" style="color: ${statusColor}; font-weight: 600;">${status}</span></td>
                <td style="display: flex; gap: 8px;">
                    <button class="btn btn-secondary edit-vendor-btn" style="padding: 6px 12px; font-size: 12px;">Edit</button>
                    <button class="btn delete-btn" style="padding: 6px 12px; font-size: 12px; background: transparent; border: 1px solid var(--status-nogo); color: var(--status-nogo);">Delete</button>
                </td>
            `;
            
            // Append and close
            tbody.appendChild(newRow);
            showToast(`${name} successfully added to the network.`, "success");
            addModal.style.display = 'none';
            addForm.reset(); 
        });
    }
}

// ==========================================
// 31. MODULE: HEURISTIC & PREDICTIVE ENGINE
// ==========================================
function initPredictiveEngine() {
    // 1. Heuristic Affinity Mapping (Mockup for image_baa94e.png)
    const affinityData = [
        { cluster: "Cluster A (Quiet/Nature)", members: "Juan M., Maria C.", cohesion: "92%", trait: "Photography" },
        { cluster: "Cluster B (High-Energy)", members: "Althea B., David T.", cohesion: "88%", trait: "Vlogging" }
    ];

    // 2. Trend Forecasting (Holy Week/Holiday Logic)
    const upcomingHolidays = ["Holy Week (April 2026)", "Independence Day (June 2026)"];
    
    // Simulate an alert if a tour falls on these dates
    const dashboardAlerts = document.getElementById('system-alerts');
    if (dashboardAlerts) {
        dashboardAlerts.innerHTML += `
            <div class="alert-box status-watch">
                <strong>Trend Alert:</strong> High demand predicted for April 
                due to ${upcomingHolidays[0]}. Suggest deploying additional transport.
            </div>
        `;
    }
}

// ==========================================
// 32. MODULE: TOUR CREATION (MOCK PERSISTENCE)
// ==========================================
function initTourCreation() {
    const createForm = document.getElementById('create-tour-form');
    const tourTableBody = document.querySelector('.data-table tbody');

    // --- PART A: Saving data from create-tour.html ---
    if (createForm) {
        createForm.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const submitBtn = createForm.querySelector('button[type="submit"]');
            submitBtn.innerText = "Publishing Tour...";
            submitBtn.disabled = true;

            // Grab the values from the form inputs
            const name = document.getElementById('tour-name').value;
            const startDate = document.getElementById('start-date').value;
            const endDate = document.getElementById('end-date').value;
            const capacity = document.getElementById('tour-capacity').value;

            // Format dates (e.g., "2026-12-01" to "Dec 01, 2026")
            const formatDate = (dateStr) => {
                const d = new Date(dateStr);
                return d.toLocaleDateString('en-US', { month: 'short', day: '2-digit', year: 'numeric' });
            };

            // Create a JSON object and save it to the browser's local storage
            const newTourData = {
                name: name,
                dates: `${formatDate(startDate)} - ${formatDate(endDate)}`,
                capacity: `0 / ${capacity}` // Starts with 0 passengers booked
            };
            
            localStorage.setItem('newlyCreatedTour', JSON.stringify(newTourData));

            showToast('Tour configuration saved successfully.', 'success');
            
            // Route back to the management table
            setTimeout(() => {
                window.location.href = 'tour-management.html';
            }, 1000);
        });
    }

    // --- PART B: Loading data into tour-management.html ---
    if (window.location.pathname.includes("tour-management.html") && tourTableBody) {
        // Check if there is a new tour sitting in local storage
        const savedTourString = localStorage.getItem('newlyCreatedTour');
        
        if (savedTourString) {
            const newTour = JSON.parse(savedTourString);
            
            // Create a brand new table row
            const newRow = document.createElement('tr');
            newRow.innerHTML = `
                <td class="heading-md">${newTour.name}</td>
                <td class="body-text">${newTour.dates}</td>
                <td class="body-text">${newTour.capacity}</td>
                <td><span class="badge" style="background-color: rgba(34, 197, 94, 0.2); color: var(--status-go);">Active</span></td>
                
                <td style="display: flex; gap: 8px;">
                    <button class="btn btn-secondary edit-tour-btn" style="padding: 6px 12px; font-size: 12px;">Edit</button>
                    <button class="btn delete-btn" style="padding: 6px 12px; font-size: 12px; background: transparent; border: 1px solid var(--status-nogo); color: var(--status-nogo);">Delete</button>
                </td>
            `;
            
            // Insert the new row at the very top of the table
            tourTableBody.insertBefore(newRow, tourTableBody.firstChild);
            
            // Clear the local storage so it doesn't duplicate if they refresh the page!
            localStorage.removeItem('newlyCreatedTour');
        }
    }
}

// ==========================================
// 33. MODULE: GUEST BOOKING FORM & HEURISTICS
// ==========================================
function initBookingForm() {

    if (localStorage.getItem('isAgencyBooking') === 'true') {
        const pageTitle = document.querySelector('.heading-xl');
        const subtitle = document.querySelector('.body-text');
        
        if (pageTitle) {
            pageTitle.innerHTML = 'Passenger Details <span class="badge" style="background-color: var(--accent-blue); color: white; vertical-align: middle; margin-left: 12px; font-size: 14px;">Agency Mode</span>';
        }
        if (subtitle) {
            subtitle.innerText = "Please provide your client's details. Commissions will be tracked upon successful payment.";
        }
    }

    const form = document.getElementById('passenger-manifest-form');
    if (!form) return; 

    // 1. Setup Price Elements
    const subtotalEl = document.getElementById('summary-subtotal');
    const discountRow = document.getElementById('discount-row');
    const discountEl = document.getElementById('summary-discount');
    const totalEl = document.getElementById('summary-total');
    
    // Make sure your HTML div uses id="companion-list" to match this!
    const addCompanionBtn = document.getElementById('add-companion-btn');
    const companionList = document.getElementById('companion-container');

    // 2. Setup Seat Limit Logic
    const totalSeatsBooked = parseInt(localStorage.getItem('checkoutSeats')) || 1;
    const maxCompanionsAllowed = totalSeatsBooked - 1; 
    let currentCompanions = 0;

    if (maxCompanionsAllowed === 0 && addCompanionBtn) {
        addCompanionBtn.style.display = 'none';
    }

    // 3. Financial Logic
    const savedTotalStr = localStorage.getItem('checkoutTotal') || "₱3,500";
    const baseTotal = parseInt(savedTotalStr.replace(/[^\d]/g, '')) || 3500;
    
    // Calculate the base price per seat (Total / Seats Booked)
    const pricePerSeat = baseTotal / totalSeatsBooked; 
    let currentFinalTotal = baseTotal;

    function updatePrice() {
        if (subtotalEl) subtotalEl.innerText = `₱${baseTotal.toLocaleString()}`;

        const allSelects = document.querySelectorAll('.discount-select');
        let totalDiscountAmt = 0;

        allSelects.forEach(select => {
            if (select.value === 'senior' || select.value === 'pwd') {
                totalDiscountAmt += (pricePerSeat * 0.20); // 20% off
            } else if (select.value === 'minor') {
                totalDiscountAmt += (pricePerSeat * 0.10); // 10% off
            }
        });

        currentFinalTotal = baseTotal - totalDiscountAmt;

        if (totalDiscountAmt > 0) {
            if (discountRow) discountRow.style.display = 'flex';
            if (discountEl) discountEl.innerText = `- ₱${totalDiscountAmt.toLocaleString()}`;
        } else {
            if (discountRow) discountRow.style.display = 'none';
        }

        if (totalEl) totalEl.innerText = `₱${currentFinalTotal.toLocaleString()}`;
    }

    // 4. Listeners
    const leadCategory = document.getElementById('lead-category');
    if (leadCategory) leadCategory.addEventListener('change', updatePrice);

    if (addCompanionBtn && companionList) {
        addCompanionBtn.addEventListener('click', (e) => {
            e.preventDefault();

            // ENFORCE SEAT LIMIT
            if (currentCompanions >= maxCompanionsAllowed) {
                showToast(`Seat limit reached! You only booked ${totalSeatsBooked} total seats.`, 'error');
                return;
            }

            const row = document.createElement('div');
            row.className = 'form-grid';
            row.style.borderTop = '1px dashed var(--border-color)';
            row.style.paddingTop = '16px';
            row.style.marginTop = '16px';

            row.innerHTML = `
                <div>
                    <label class="label-text">Companion Name</label>
                    <input type="text" class="input-field" required style="margin-top: 8px;">
                </div>
                <div>
                    <label class="label-text">Age</label>
                    <input type="number" class="input-field" required style="margin-top: 8px;">
                </div>
                <div>
                    <label class="label-text">Category</label>
                    <select class="input-field discount-select" style="margin-top: 8px;">
                        <option value="none">None (Regular)</option>
                        <option value="senior">Senior Citizen (20% Off)</option>
                        <option value="pwd">PWD (20% Off)</option>
                        <option value="minor">Student/Minor (10% Off)</option>
                    </select>
                </div>
                <div style="display: flex; align-items: flex-end;">
                    <button type="button" class="btn remove-btn" style="background: transparent; color: var(--status-nogo); border: 1px solid var(--status-nogo); width: 100%; padding: 12px; cursor: pointer;">Remove</button>
                </div>
            `;

            row.querySelector('.discount-select').addEventListener('change', updatePrice);
            
            row.querySelector('.remove-btn').addEventListener('click', () => {
                row.remove();
                currentCompanions--; // Free up a companion slot
                updatePrice(); // Recalculate price when companion is removed
                
                // Unlock the button
                addCompanionBtn.style.opacity = '1';
                addCompanionBtn.style.cursor = 'pointer';
            });

            companionList.appendChild(row);
            currentCompanions++; // Take up a companion slot

            // Lock the button if max is reached
            if (currentCompanions >= maxCompanionsAllowed) {
                addCompanionBtn.style.opacity = '0.5';
                addCompanionBtn.style.cursor = 'not-allowed';
            }

            updatePrice(); 
        });
    }

    updatePrice(); // Initial calculation

    // 5. Form Submit Handler
    form.addEventListener('submit', (e) => {
        e.preventDefault(); 
        
        // Save the FINAL discounted total for the next page
        localStorage.setItem('checkoutTotal', `₱${currentFinalTotal.toLocaleString()}`);
        
        const submitBtn = form.querySelector('button[type="submit"]');
        submitBtn.innerText = "Finalizing Invoice...";
        submitBtn.disabled = true;

        showToast(`Manifest saved. Redirecting to payment...`, 'success');

        setTimeout(() => {
            window.location.href = 'payment-upload.html';
        }, 1200);
    });
}

// ==========================================
// 34. MODULE: TRACK BOOKING (GUEST PORTAL)
// ==========================================
function initTrackBooking() {
    const form = document.getElementById('track-booking-form');
    if (!form) return;

    form.addEventListener('submit', (e) => {
        e.preventDefault();

        const submitBtn = form.querySelector('button[type="submit"]');
        const refInput = document.getElementById('ref-number').value;
        
        submitBtn.innerText = "Searching Database...";
        submitBtn.disabled = true;

        // Simulate database lookup delay
        setTimeout(() => {
            // Capstone Mockup Magic: We accept any input here to keep the demo smooth, 
            // but in a real app, this would query your PHP/MySQL backend!
            showToast(`Booking ${refInput.toUpperCase()} found! Loading status...`, 'success');
            
            setTimeout(() => {
                // Route them directly to the status page!
                window.location.href = 'booking-status-detail.html';
            }, 1000);

        }, 1200);
    });
}

// ==========================================
// 35. MODULE: BOOKING CONFIRMATION
// ==========================================
function initBookingConfirmation() {
    const tourNameDisplay = document.getElementById('confirm-tour-name');
    const amountDisplay = document.getElementById('confirm-amount');
    const refDisplay = document.getElementById('confirm-ref-number');

    // 1. Retrieve the saved data from the previous steps
    const savedTourName = localStorage.getItem('bookedTourName');
    const savedTotal = localStorage.getItem('checkoutTotal');

    // 2. Inject the dynamic Tour Name
    if (savedTourName && tourNameDisplay) {
        tourNameDisplay.innerText = savedTourName;
    }

    // 3. Inject the dynamic Final Price
    if (savedTotal && amountDisplay) {
        amountDisplay.innerText = savedTotal;
    }

    // 4. Generate a random Reference Number for realism (e.g., #JH-48291)
    if (refDisplay) {
        const randomNum = Math.floor(10000 + Math.random() * 90000);
        refDisplay.innerText = `#JH-${randomNum}`;
    }
}

// ==========================================
// 36. MODULE: BOOKING STATUS DETAIL
// ==========================================
function initBookingStatusDetail() {
    console.log("1. Status detail module started!");
    
    const dataStr = localStorage.getItem('trackedBooking');
    console.log("2. Data found in memory:", dataStr);

    if (!dataStr) {
        console.error("ERROR: No data found in localStorage! The track booking page didn't save it.");
        return; 
    }

    const data = JSON.parse(dataStr);

    const refEl = document.getElementById('status-ref-number');
    const nameEl = document.getElementById('status-tour-name');
    const dateEl = document.getElementById('status-date');
    const amountEl = document.getElementById('status-amount');
    const badgeEl = document.getElementById('status-badge');

    if (!nameEl) {
        console.error("ERROR: Could not find the id='status-tour-name' in the HTML!");
        return;
    }

    // Inject the data
    if (refEl) refEl.innerText = `Booking #${data.ref}`;
    if (nameEl) nameEl.innerText = data.name;
    if (dateEl) dateEl.innerText = data.date;
    if (amountEl) amountEl.innerText = data.amount;
    
    if (badgeEl) {
        badgeEl.innerText = data.status;
        badgeEl.style.backgroundColor = data.bgColor;
        badgeEl.style.color = data.textColor;
    }

    console.log("3. UI successfully updated!");
}

// ==========================================
// 37. MODULE: BOOKING TRACKING VALIDATION
// ==========================================
function initTrackBooking() {
    // Look for the primary button on the tracking page
    const findBtn = document.querySelector('button'); 
    // Grab the input fields (assuming the Ref Number is the first one)
    const inputs = document.querySelectorAll('.input-field');

    if (findBtn && inputs.length > 0) {
        findBtn.addEventListener('click', (e) => {
            e.preventDefault(); // Stop the page from immediately navigating
            
            const originalText = findBtn.innerText;
            const refNumber = inputs[0].value.trim().toUpperCase();
            
            findBtn.innerText = "Searching Database...";
            findBtn.disabled = true;
            findBtn.style.opacity = "0.7";

            setTimeout(() => {
                // THE ERROR HANDLING LOGIC
                // Only allow them through if they type your exact Sagada mockup ID
                if (refNumber !== "JH-99824") {
                    
                    showToast(`Error: No booking found for reference ${refNumber || 'EMPTY'}.`, 'error');
                    
                    // Reset the button so they can try again
                    findBtn.innerText = originalText;
                    findBtn.disabled = false;
                    findBtn.style.opacity = "1";
                    
                    // Highlight the input box in red to show an error
                    inputs[0].style.borderColor = "var(--status-nogo)";
                    
                } else {
                    // Success Path!
                    showToast('Booking found. Retrieving itinerary...', 'success');
                    
                    setTimeout(() => {
                        window.location.href = 'booking-status-detail.html';
                    }, 800);
                }
            }, 600); // Simulate network delay
        });
        
        // Remove the red border when the user starts typing again
        inputs[0].addEventListener('input', () => {
            inputs[0].style.borderColor = "var(--border-color)";
        });
    }
}

// ==========================================
// 38. MODULE: INJECT CARD IMAGES (DISCOVERY PAGE)
// ==========================================
function initTourDiscoveryImages() {
    const cardImages = {
        "sagada": "assets/Sagada-44.jpg",
        "pulag": "assets/pulag.jpg",
        "batanes": "assets/Batanes-1-1600x1066.jpg",
        "siargao": "assets/siargao.jpg"
    };

    const cards = document.querySelectorAll('.tour-card');
    
    cards.forEach(card => {
        // Find the "View Details" button link
        const linkBtn = card.querySelector('a');
        
        if (linkBtn) {
            const href = linkBtn.getAttribute('href');
            
            // Extract the ID safely (e.g., from "?id=pulag" get "pulag")
            if (href && href.includes('id=')) {
                const id = href.split('id=')[1];
                
                // If we have an image for this ID, apply it to the container!
                if (cardImages[id]) {
                    const imgContainer = card.querySelector('.tour-image');
                    if (imgContainer) {
                        imgContainer.style.backgroundImage = `url('${cardImages[id]}')`;
                        imgContainer.style.backgroundSize = "cover";
                        imgContainer.style.backgroundPosition = "center";
                    }
                }
            }
        }
    });
}

// ==========================================
// MODULE: COMMISSION TRACKER (CSV EXPORT)
// ==========================================
function initCommissionTracker() {
    const exportBtn = document.getElementById('export-csv-btn');
    const tableBody = document.getElementById('commission-table-body');

    if (exportBtn && tableBody) {
        exportBtn.addEventListener('click', () => {
            // 1. Setup the CSV Headers
            let csvContent = "Client/Ref,Tour,Commission Amount,Status\n";

            // 2. Loop through all rows in the table
            const rows = tableBody.querySelectorAll('tr');
            rows.forEach(row => {
                const cols = row.querySelectorAll('td');
                if (cols.length > 0) {
                    // Extract text and clean up commas so it doesn't break the CSV format
                    const client = cols[0].innerText.replace(/,/g, '').trim();
                    const tour = cols[1].innerText.replace(/,/g, '').trim();
                    const amount = cols[2].innerText.replace(/,/g, '').trim();
                    const status = cols[3].innerText.replace(/,/g, '').trim();

                    // Append the row to our CSV string
                    csvContent += `${client},${tour},${amount},${status}\n`;
                }
            });

            // 3. Create a Blob (Binary Large Object) to hold the file data
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            
            // 4. Create a temporary hidden link to force the browser download
            const link = document.createElement("a");
            const url = URL.createObjectURL(blob);
            
            link.setAttribute("href", url);
            // Name the file dynamically with today's date!
            link.setAttribute("download", `J-Hub_Commission_Ledger_${new Date().toISOString().split('T')[0]}.csv`);
            link.style.visibility = 'hidden';
            
            // Execute the download
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            // Give the user a success message
            showToast("Ledger successfully exported as CSV!", "success");
        });
    }
}

// ==========================================
// MODULE: AGENCY CATALOG (FILTER & B2B BRIDGE)
// ==========================================
function initAgencyCatalog() {
    // 1. DYNAMIC TABLE FILTERING
    const searchInput = document.getElementById('agency-search-input');
    const dateFilter = document.getElementById('agency-date-filter');
    const filterBtn = document.getElementById('agency-filter-btn');
    const tableBody = document.getElementById('agency-catalog-body');

    function applyFilters() {
        if (!tableBody) return;
        const searchTerm = searchInput.value.toLowerCase();
        const dateTerm = dateFilter.value;

        const rows = tableBody.querySelectorAll('tr');
        rows.forEach(row => {
            const rowText = row.innerText.toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesDate = (dateTerm === "All Dates") || rowText.includes(dateTerm.toLowerCase());

            if (matchesSearch && matchesDate) {
                row.style.display = ""; // Show
            } else {
                row.style.display = "none"; // Hide
            }
        });
    }

    if (filterBtn) filterBtn.addEventListener('click', applyFilters);
    if (searchInput) searchInput.addEventListener('input', applyFilters);
    if (dateFilter) dateFilter.addEventListener('change', applyFilters);


    // 2. THE B2B BOOKING BRIDGE
    const bookButtons = document.querySelectorAll('.book-client-btn');
    bookButtons.forEach(btn => {
        btn.addEventListener('click', (e) => {
            const tourId = e.target.getAttribute('data-id');
            const tourName = e.target.getAttribute('data-tour');
            const tourPrice = e.target.getAttribute('data-price');

            // Save details to bypass the tour-details page and go straight to the form
            localStorage.setItem('bookedTourName', tourName);
            localStorage.setItem('checkoutTotal', `₱${parseInt(tourPrice).toLocaleString()}`);
            localStorage.setItem('checkoutSeats', 1);
            
            // SET THE SECRET AGENCY FLAG
            localStorage.setItem('isAgencyBooking', 'true');

            showToast(`Initializing B2B booking for ${tourName}...`, 'info');
            e.target.innerText = "Routing...";
            e.target.disabled = true;

            setTimeout(() => {
                window.location.href = 'booking-form.html';
            }, 800);
        });
    });
}

// ==========================================
// MODULE: CLIENT MANAGEMENT (POP MODAL)
// ==========================================
function initClientManagement() {
    const uploadButtons = document.querySelectorAll('.upload-pop-btn');
    const modal = document.getElementById('pop-modal');
    const closeBtn = document.getElementById('close-modal-btn');
    const submitBtn = document.getElementById('submit-pop-btn');
    const fileInput = document.getElementById('pop-file-input');
    const clientNameSpan = document.getElementById('modal-client-name');
    
    let activeRow = null; // Keeps track of which client we are updating

    if (modal && uploadButtons.length > 0) {
        // 1. Open Modal when an agency clicks "Upload PoP"
        uploadButtons.forEach(btn => {
            btn.addEventListener('click', (e) => {
                // Find the table row that contains this button
                activeRow = e.target.closest('tr');
                
                // Grab the client's name from the first column of the row
                const clientName = activeRow.querySelector('td').innerText;
                clientNameSpan.innerText = clientName;

                // Show the modal
                modal.style.display = 'flex';
            });
        });

        // 2. Close Modal
        closeBtn.addEventListener('click', () => {
            modal.style.display = 'none';
            fileInput.value = ""; // Reset the file input
        });

        // 3. Handle Submit
        submitBtn.addEventListener('click', () => {
            if (!fileInput.value) {
                showToast("Please select a file to upload.", "error");
                return;
            }

            // Simulate the upload process
            submitBtn.innerText = "Uploading...";
            submitBtn.disabled = true;

            setTimeout(() => {
                // Change the yellow "Pending PoP" badge to a blue "Under Review" badge
                const statusCell = activeRow.querySelectorAll('td')[2]; // Assuming status is the 3rd column
                if (statusCell) {
                    statusCell.innerHTML = `<span class="badge" style="background-color: rgba(59, 130, 246, 0.2); color: var(--accent-blue);">Under Review</span>`;
                }

                // Change the button text
                const actionBtn = activeRow.querySelector('.upload-pop-btn');
                if (actionBtn) {
                    actionBtn.innerText = "View Receipt";
                    actionBtn.className = "btn btn-secondary";
                    actionBtn.style.background = "transparent";
                    actionBtn.style.border = "1px solid var(--border-color)";
                    actionBtn.disabled = true; // Disable until reviewed by operator
                }

                // Reset and close modal
                showToast("Proof of Payment uploaded successfully!", "success");
                submitBtn.innerText = "Submit PoP";
                submitBtn.disabled = false;
                fileInput.value = "";
                modal.style.display = 'none';

            }, 1200); // 1.2 second fake loading delay
        });
    }
}

// ==========================================
// MODULE: AGENCY DASHBOARD (ANIMATIONS)
// ==========================================
function initAgencyDashboard() {
    // 1. Animate Progress Bars
    // We use a slight timeout so the animation happens AFTER the page is fully drawn
    setTimeout(() => {
        const progressBars = document.querySelectorAll('.progress-bar-fill');
        progressBars.forEach(bar => {
            const targetWidth = bar.getAttribute('data-target-width');
            bar.style.width = targetWidth;
        });
    }, 200);

    // 2. Animate Number Counters
    const counters = document.querySelectorAll('.stat-counter');
    const animationDuration = 1500; // 1.5 seconds

    counters.forEach(counter => {
        const targetValue = parseFloat(counter.getAttribute('data-target'));
        const isCurrency = counter.hasAttribute('data-currency');
        
        let startTime = null;

        // Using requestAnimationFrame is the smoothest way to animate numbers in JS
        function updateCounter(currentTime) {
            if (!startTime) startTime = currentTime;
            const progress = currentTime - startTime;
            
            // Calculate how far along we are (from 0 to 1)
            const percentage = Math.min(progress / animationDuration, 1);
            
            // Easing function to make it slow down at the end
            const easeOutQuart = 1 - Math.pow(1 - percentage, 4);
            const currentValue = targetValue * easeOutQuart;

            if (isCurrency) {
                // Format with Peso sign, commas, and 2 decimal places
                counter.innerText = '₱' + currentValue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            } else {
                // Whole numbers for bookings
                counter.innerText = Math.floor(currentValue);
            }

            // Keep looping until the animation duration is hit
            if (progress < animationDuration) {
                window.requestAnimationFrame(updateCounter);
            } else {
                // Ensure the final number is perfectly exact
                if (isCurrency) {
                    counter.innerText = '₱' + targetValue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                } else {
                    counter.innerText = targetValue;
                }
            }
        }

        window.requestAnimationFrame(updateCounter);
    });
}

// ==========================================
// MODULE: OPERATOR SYSTEM SETTINGS
// ==========================================
function initSystemSettings() {
    const saveBtn = document.querySelector('.btn-primary'); // Assuming it's the only primary button
    const inputs = document.querySelectorAll('.input-field');
    
    // Load existing settings if they exist
    inputs.forEach((input, index) => {
        const savedValue = localStorage.getItem(`sys_setting_${index}`);
        if(savedValue) input.value = savedValue;
    });

    if(saveBtn) {
        saveBtn.addEventListener('click', () => {
            saveBtn.innerText = "Saving...";
            saveBtn.disabled = true;
            
            setTimeout(() => {
                inputs.forEach((input, index) => {
                    localStorage.setItem(`sys_setting_${index}`, input.value);
                });
                
                showToast("Global configurations updated successfully.", "success");
                saveBtn.innerText = "Save Configurations";
                saveBtn.disabled = false;
            }, 800);
        });
    }
}

// ==========================================
// MODULE: OPERATOR REPORTS
// ==========================================
function initReports() {
    const exportBtn = document.getElementById('manifest-csv-btn');
    
    if(exportBtn) {
        exportBtn.addEventListener('click', (e) => {
            e.preventDefault();
            const selectTour = document.querySelector('select').value;
            
            // Generate a dummy CSV for the demo
            let csvContent = "Passenger Name,Contact,Status,Seat Number\n";
            csvContent += "Juan Dela Cruz,09171234567,Paid,1\n";
            csvContent += "Maria Clara,09189876543,Paid,2\n";
            
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const link = document.createElement("a");
            link.setAttribute("href", URL.createObjectURL(blob));
            link.setAttribute("download", `Passenger_Manifest.csv`);
            link.style.visibility = 'hidden';
            
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            
            showToast("Passenger Manifest exported successfully.", "success");
        });
    }
}

// ==========================================
// MODULE: OPERATOR NOTIFICATIONS
// ==========================================
function initNotifications() {
    const markReadBtn = document.getElementById('mark-read-btn');
    
    if(markReadBtn) {
        markReadBtn.addEventListener('click', (e) => {
            e.preventDefault();
            
            // Dim all the notification cards
            const notificationCards = document.querySelectorAll('.modal-content, .tour-card, [style*="border: 1px solid var(--border-color)"]'); 
            
            notificationCards.forEach(card => {
                card.style.opacity = '0.5';
                const icon = card.querySelector('svg'); // Target the icons directly
                if(icon) icon.style.display = 'none';
            });
            
            // Clear the red badge on the sidebar
            const sidebarBadge = document.querySelector('.sidebar .badge');
            if(sidebarBadge) sidebarBadge.style.display = 'none';
            
            showToast("All notifications marked as read.", "info");
        });
    }
}

// ==========================================
// MODULE: TOUR MANAGEMENT (OPERATOR)
// ==========================================
function initTourManagement() {
    const tourTable = document.getElementById('tour-table');
    const editModal = document.getElementById('edit-tour-modal');
    const closeEditBtn = document.getElementById('close-edit-tour-btn');
    const editForm = document.getElementById('edit-tour-form');
    let activeEditRow = null;

    // 1. EVENT DELEGATION (Handles Delete and Edit clicks)
    if (tourTable) {
        tourTable.addEventListener('click', (e) => {
            
            // DELETE LOGIC
            if (e.target.classList.contains('delete-btn')) {
                const row = e.target.closest('tr');
                const tourName = row.querySelector('td').innerText; 
                if(confirm(`Are you sure you want to delete "${tourName}"? This will cancel any connected bookings.`)) {
                    row.remove();
                    showToast(`Tour deleted successfully.`, "success");
                }
            }
            
            // OPEN EDIT MODAL LOGIC
            if (e.target.classList.contains('edit-tour-btn')) {
                activeEditRow = e.target.closest('tr');
                const cols = activeEditRow.querySelectorAll('td');
                
                const name = cols[0].innerText;
                const currentStatus = cols[3].innerText.trim();

                // Set Modal Name
                document.getElementById('edit-tour-name-display').innerText = name;
                
                // Pre-fill Status Dropdown
                const statusSelect = document.getElementById('edit-tour-status');
                Array.from(statusSelect.options).forEach(opt => {
                    if (opt.value.includes(currentStatus)) opt.selected = true;
                });

                editModal.style.display = 'flex';
            }
        });
    }

    // 2. SAVE STATUS EDIT
    if (editForm) {
        editForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const newStatusSelect = document.getElementById('edit-tour-status');
            // Extract just the first word (e.g., "Active" from "Active (Visible to Joiners)")
            const newStatus = newStatusSelect.options[newStatusSelect.selectedIndex].text.split(' ')[0];

            let badgeBg, badgeColor;
            
            if (newStatus === 'Active') {
                badgeBg = 'rgba(34, 197, 94, 0.2)';
                badgeColor = 'var(--status-go)';
            } else if (newStatus === 'Pending') {
                badgeBg = 'rgba(234, 179, 8, 0.2)';
                badgeColor = 'var(--status-watch)';
            } else if (newStatus === 'Completed') {
                badgeBg = 'rgba(255, 255, 255, 0.1)';
                badgeColor = 'var(--text-muted)';
            } else { // Cancelled
                badgeBg = 'rgba(239, 68, 68, 0.2)';
                badgeColor = 'var(--status-nogo)';
            }

            activeEditRow.querySelectorAll('td')[3].innerHTML = `<span class="badge" style="background-color: ${badgeBg}; color: ${badgeColor};">${newStatus}</span>`;

            editModal.style.display = 'none';
            showToast("Tour status updated successfully.", "success");
        });
    }

    if (closeEditBtn) closeEditBtn.addEventListener('click', () => editModal.style.display = 'none');
}

// ==========================================
// GLOBAL FUNCTION: SMART BACK BUTTON
// ==========================================
function smartBack() {
    // 1. Log the current URL
    const currentUrl = window.location.href;
    
    // 2. Attempt to go back using the browser's history
    window.history.back();

    // 3. If the URL hasn't changed after 100 milliseconds, it means history.back() failed (e.g., opened in a new tab)
    setTimeout(() => {
        if (window.location.href === currentUrl) {
            // The fallback: Send them to the Operator Dashboard if there is no history
            window.location.href = 'operator-dashboard.html'; 
        }
    }, 100);
}