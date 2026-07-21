    (function () {
        'use strict';

    document.addEventListener('DOMContentLoaded', init);

    function init() {
            if (!window.jQuery) {
        console.error('jQuery is not loaded. Statement portal script cannot run.');
    return;
            }
    var $ = window.jQuery;

    // Initialize date pickers
    if (window.flatpickr) {
        flatpickr('#StartDate', { dateFormat: 'Y-m-d' });
    flatpickr('#EndDate', {dateFormat: 'Y-m-d' });
            }

    // ---------------------------------------------------------------
    // State
    // ---------------------------------------------------------------
    var state = {
        customerAccounts: [],
    pending: {
        accounts: [],
    fileNamePrefix: ''
                },
    pendingTempFilePath: null // Store temp file path from preview
            };

    // ---------------------------------------------------------------
    // Element cache
    // ---------------------------------------------------------------
    var els = {
        form: document.getElementById('searchForm'),
    searchBtn: document.getElementById('searchBtn'),
    btnText: document.getElementById('btn-text'),
    btnLoader: document.getElementById('btn-loader'),
    searchError: document.getElementById('searchError'),
    resultsSection: document.getElementById('resultsSection'),
    customerTable: document.getElementById('customerTable'),
    searchParam: document.getElementById('SearchParameter'),
    searchValue: document.getElementById('SearchValue'),
    startDate: document.getElementById('StartDate'),
    endDate: document.getElementById('EndDate'),

    downloadPdfBtn: document.getElementById('downloadPdfBtn'),
    downloadExcelBtn: document.getElementById('downloadExcelBtn'),
    emailBtn: document.getElementById('emailBtn'),

    pdfModal: document.getElementById('pdfModal'),
    pdfTableBody: document.querySelector('#pdfTable tbody'),
    selectAllPdf: document.getElementById('selectAllPdf'),
    downloadSelectedPdf: document.getElementById('downloadSelectedPdf'),
    downloadAllPdf: document.getElementById('downloadAllPdf'),

    chargeModal: document.getElementById('chargeConfirmModal'),
    chargeAmount: document.getElementById('chargeAmount'),
    pageCount: document.getElementById('pageCount'),
    chargeAccountCount: document.getElementById('chargeAccountCount'),
    chargePeriod: document.getElementById('chargePeriod'),
    confirmCharge: document.getElementById('confirmChargeBtn'),
    cancelCharge: document.getElementById('cancelChargeBtn'),
    chargeNote: document.querySelector('#chargeConfirmModal .text-gray-700 p:last-child'),

    excelModal: document.getElementById('excelModal'),
    excelTableBody: document.querySelector('#excelTable tbody'),
    selectAllExcel: document.getElementById('selectAllExcel'),
    downloadSelectedExcel: document.getElementById('downloadSelectedExcel'),
    downloadAllExcel: document.getElementById('downloadAllExcel'),

    emailModal: document.getElementById('emailModal'),
    emailTableBody: document.querySelector('#emailTable tbody'),
    selectAllEmail: document.getElementById('selectAllEmail'),
    emailSelectedBtn: document.getElementById('emailSelectedBtn'),
    emailAllBtn: document.getElementById('emailAllBtn'),

    successModal: document.getElementById('successModal'),
    successMessage: document.getElementById('successMessage'),
    errorModal: document.getElementById('errorModal'),
    errorMessage: document.getElementById('errorMessage')
            };

    // ---------------------------------------------------------------
    // Modal helper functions
    // ---------------------------------------------------------------
    function showModal(el) {
                if (el) {
        el.style.display = 'flex';
    el.style.alignItems = 'center';
    el.style.justifyContent = 'center';
                }
            }

    function hideModal(el) {
                if (el) el.style.display = 'none';
            }

    function hideAllModals() {
        document.querySelectorAll('.modal').forEach(hideModal);
            }

    // Close modals with proper event handling
    document.addEventListener('click', function (e) {
                if (e.target.classList && e.target.classList.contains('close-modal')) {
                    var modal = e.target.closest('.modal');
    if (modal) hideModal(modal);
    e.preventDefault();
                }
    if (e.target.classList && e.target.classList.contains('modal')) {
        hideModal(e.target);
                }
            });

    document.addEventListener('keydown', function (e) {
                if (e.key === 'Escape') hideAllModals();
            });

    // jQuery close handlers
    $(document).on('click', '.close-modal', function () {
                var modal = $(this).closest('.modal');
    if (modal.length) modal.hide();
            });

    $(document).on('click', '.modal', function (e) {
                if (e.target === this) {
        $(this).hide();
                }
            });

    // ---------------------------------------------------------------
    // Small helpers
    // ---------------------------------------------------------------
    function setBusy(btn, busy, busyLabel) {
                if (!btn) return;
    if (busy) {
                    if (!btn.dataset.originalHtml) btn.dataset.originalHtml = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> ' + (busyLabel || 'Processing...');
    btn.disabled = true;
                } else {
                    if (btn.dataset.originalHtml) {
        btn.innerHTML = btn.dataset.originalHtml;
    delete btn.dataset.originalHtml;
                    }
    btn.disabled = false;
                }
            }

    function dedupeByAccountNumber(accounts) {
                var seen = { };
    return (accounts || []).filter(function (a) {
                    var key = a.AccountNumber || a.accountNumber || a.accountnumber;
    if (!key) return true;
    if (seen[key]) return false;
    seen[key] = true;
    return true;
                });
            }

    function safeGet(obj, prop, fallback) {
                if (!obj) return fallback || '';
    if (obj[prop] !== undefined && obj[prop] !== null) return obj[prop];
    var lowerProp = prop.toLowerCase();
    for (var key in obj) {
                    if (obj.hasOwnProperty(key) && key.toLowerCase() === lowerProp) {
                        return obj[key] || fallback || '';
                    }
                }
    return fallback || '';
            }

    function escapeHtml(value) {
                return String(value == null ? '' : value)
    .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
            }

function money(value) {
    var n = parseFloat(value || 0);
    return isNaN(n) ? value : n.toLocaleString();
}

function showSuccess(message) {
    if (els.successModal) {
        els.successMessage.textContent = message;
        showModal(els.successModal);
    } else {
        alert(message);
    }
}

function showError(message) {
    if (els.errorModal) {
        els.errorMessage.textContent = message;
        showModal(els.errorModal);
    } else {
        alert(message);
    }
}

function normalizeAccount(acc) {
    if (!acc) return null;
    return {
        CIF_ID: safeGet(acc, 'CIF_ID') || safeGet(acc, 'CifId') || safeGet(acc, 'cif_id'),
        AccountNumber: safeGet(acc, 'AccountNumber') || safeGet(acc, 'accountNumber') || safeGet(acc, 'accountnumber'),
        Balance: safeGet(acc, 'Balance') || safeGet(acc, 'balance') || 0,
        DisplayName: safeGet(acc, 'DisplayName') || safeGet(acc, 'displayName') || safeGet(acc, 'displayname') || safeGet(acc, 'CustomerName') || safeGet(acc, 'customerName'),
        AccountCurrency: safeGet(acc, 'AccountCurrency') || safeGet(acc, 'accountCurrency') || safeGet(acc, 'accountcurrency') || safeGet(acc, 'Currency') || safeGet(acc, 'currency'),
        BranchCode: safeGet(acc, 'BranchCode') || safeGet(acc, 'branchCode') || 0,
        ProductType: safeGet(acc, 'ProductType') || safeGet(acc, 'productType') || safeGet(acc, 'producttype'),
        ProductCode: safeGet(acc, 'ProductCode') || safeGet(acc, 'productCode') || safeGet(acc, 'productcode'),
        Email: safeGet(acc, 'Email') || safeGet(acc, 'email')
    };
}

function buildAccountPayload(acc) {
    var normalized = normalizeAccount(acc);
    return {
        AccountNumber: normalized.AccountNumber || '',
        AccountName: normalized.DisplayName || '',
        Balance: String(normalized.Balance || 0),
        Currency: normalized.AccountCurrency || '',
        BranchCode: normalized.BranchCode || 0,
        ProductType: normalized.ProductType || '',
        ProductCode: normalized.ProductCode || '',
        Email: normalized.Email || ''
    };
}

function buildPdfRequest(accounts) {
    var first = accounts && accounts.length ? normalizeAccount(accounts[0]) : {};
    return {
        StartDate: els.startDate.value,
        EndDate: els.endDate.value,
        CIF_ID: first.CIF_ID || '',
        actualStatementAccount: first.AccountNumber || '',
        Accounts: accounts.map(buildAccountPayload)
    };
}

// ---------------------------------------------------------------
// SEARCH
// ---------------------------------------------------------------
if (els.form) {
    els.form.addEventListener('submit', function (e) {
        e.preventDefault();

        els.searchError.classList.add('hidden');
        els.btnText.classList.add('hidden');
        els.btnLoader.classList.remove('hidden');
        els.searchBtn.disabled = true;

        var formData = new FormData();
        formData.append('SearchParameter', els.searchParam.value);
        formData.append('SearchValue', els.searchValue.value);
        formData.append('StartDate', els.startDate.value);
        formData.append('EndDate', els.endDate.value);

        var token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token) formData.append('__RequestVerificationToken', token.value);

        $.ajax({
            url: '/Home/Search',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            dataType: 'json'
        }).done(function (res) {
            console.log('Search response:', res);

            if (res && res.success && res.data && res.data.length) {
                var normalizedData = res.data.map(normalizeAccount).filter(function (a) { return a !== null; });
                console.log('Normalized data:', normalizedData);

                state.customerAccounts = dedupeByAccountNumber(normalizedData);
                renderResultsTable(state.customerAccounts);
                els.resultsSection.classList.remove('hidden');
                els.searchError.classList.add('hidden');
            } else {
                state.customerAccounts = [];
                els.resultsSection.classList.add('hidden');
                els.searchError.textContent = (res && res.message) || 'No accounts found';
                els.searchError.classList.remove('hidden');
            }
        }).fail(function (xhr) {
            console.error('Search failed:', xhr);
            state.customerAccounts = [];
            els.resultsSection.classList.add('hidden');
            var msg = 'An error occurred while searching. Please try again.';
            try {
                var response = JSON.parse(xhr.responseText);
                msg = response.message || msg;
            } catch (_) { }
            els.searchError.textContent = msg;
            els.searchError.classList.remove('hidden');
        }).always(function () {
            els.btnText.classList.remove('hidden');
            els.btnLoader.classList.add('hidden');
            els.searchBtn.disabled = false;
        });
    });
}

function renderResultsTable(data) {
    if (!data || !data.length) {
        els.customerTable.innerHTML =
            '<tr><td colspan="5" class="px-6 py-5 text-center text-gray-500">No records to display</td></tr>';
        return;
    }
    var html = '';
    data.forEach(function (item) {
        var normalized = normalizeAccount(item);
        html += '<tr class="hover:bg-gray-50">' +
            '<td class="px-6 py-5">' + escapeHtml(normalized.AccountNumber || '\u2014') + '</td>' +
            '<td class="px-6 py-5 font-medium">' + escapeHtml(normalized.DisplayName || '\u2014') + '</td>' +
            '<td class="px-6 py-5">' + escapeHtml(normalized.Email || '\u2014') + '</td>' +
            '<td class="px-6 py-5">' + money(normalized.Balance) + '</td>' +
            '<td class="px-6 py-5">' + escapeHtml(normalized.AccountCurrency || '\u2014') + '</td>' +
            '</tr>';
    });
    els.customerTable.innerHTML = html;
}

// ---------------------------------------------------------------
// Selection table renderer
// ---------------------------------------------------------------
function renderSelectionRows(tbody, checkboxClass, columns) {
    var html = '';
    var accounts = dedupeByAccountNumber(state.customerAccounts);
    accounts.forEach(function (a, idx) {
        var normalized = normalizeAccount(a);
        html += '<tr>' +
            '<td class="p-3"><input type="checkbox" class="' + checkboxClass + '" checked data-idx="' + idx + '"></td>' +
            '<td class="p-3">' + escapeHtml(normalized.AccountNumber) + '</td>' +
            '<td class="p-3">' + escapeHtml(normalized.DisplayName) + '</td>' +
            (columns.showBalance ? '<td class="p-3">' + money(normalized.Balance) + '</td>' : '') +
            '<td class="p-3">' + escapeHtml(normalized.AccountCurrency) + '</td>' +
            '</tr>';
    });
    tbody.innerHTML = html;
}

function getSelected(checkboxClass) {
    var selected = [];
    document.querySelectorAll('.' + checkboxClass + ':checked').forEach(function (cb) {
        var idx = parseInt(cb.dataset.idx, 10);
        var accounts = dedupeByAccountNumber(state.customerAccounts);
        var acc = accounts[idx];
        if (acc) selected.push(acc);
    });
    return selected;
}

function wireSelectionControls(selectAllEl, checkboxClass, primaryBtn, secondaryBtn) {
    function refresh() {
        var boxes = Array.from(document.querySelectorAll('.' + checkboxClass));
        var any = boxes.some(function (b) { return b.checked; });
        if (primaryBtn) {
            primaryBtn.disabled = !any;
            primaryBtn.classList.toggle('opacity-50', !any);
            primaryBtn.classList.toggle('cursor-not-allowed', !any);
        }
        if (selectAllEl) selectAllEl.checked = boxes.length > 0 && boxes.every(function (b) { return b.checked; });
    }
    document.querySelectorAll('.' + checkboxClass).forEach(function (cb) {
        cb.onchange = refresh;
    });
    if (selectAllEl) {
        selectAllEl.onchange = function (e) {
            document.querySelectorAll('.' + checkboxClass).forEach(function (cb) { cb.checked = e.target.checked; });
            refresh();
        };
    }
    refresh();
}

// ---------------------------------------------------------------
// PDF MODAL with Charge Preview
// ---------------------------------------------------------------
if (els.downloadPdfBtn) {
    els.downloadPdfBtn.addEventListener('click', function () {
        if (!state.customerAccounts.length) { showError('Please search for a customer first.'); return; }
        renderSelectionRows(els.pdfTableBody, 'pdf-cb', { showBalance: true });
        wireSelectionControls(els.selectAllPdf, 'pdf-cb', els.downloadSelectedPdf);
        showModal(els.pdfModal);
    });
}

if (els.downloadSelectedPdf) {
    els.downloadSelectedPdf.addEventListener('click', function () {
        var sel = getSelected('pdf-cb');
        if (!sel.length) { showError('Select at least one account.'); return; }
        hideModal(els.pdfModal);
        openChargeConfirmation(sel, 'Selected_Statements');
    });
}

if (els.downloadAllPdf) {
    els.downloadAllPdf.addEventListener('click', function () {
        hideModal(els.pdfModal);
        openChargeConfirmation(state.customerAccounts, 'All_Statements');
    });
}

// Open charge confirmation modal with preview
function openChargeConfirmation(accounts, fileNamePrefix) {
    state.pending.accounts = accounts;
    state.pending.fileNamePrefix = fileNamePrefix;
    state.pendingTempFilePath = null;

    if (els.chargeAccountCount) els.chargeAccountCount.textContent = accounts.length;
    if (els.chargePeriod) els.chargePeriod.textContent = els.startDate.value + ' to ' + els.endDate.value;

    if (els.chargeAmount) els.chargeAmount.textContent = 'Calculating...';
    if (els.pageCount) els.pageCount.textContent = '...';
    if (els.chargeNote) els.chargeNote.textContent = 'Standard statement fees apply per the bank\'s tariff. Do you want to proceed?';

    showModal(els.chargeModal);

    // Fetch charge preview (generates actual PDF)
    var payload = buildPdfRequest(accounts);

    $.ajax({
        url: '/Home/PreviewPdfCharge',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ request: JSON.stringify(payload) })
    }).done(function (resp) {
        if (resp && resp.success) {
            if (els.chargeAmount) els.chargeAmount.textContent = resp.chargeAmount || '0.00';
            if (els.pageCount) els.pageCount.textContent = resp.pageCount || '0';
            if (els.chargePeriod) els.chargePeriod.textContent = resp.period || els.startDate.value + ' to ' + els.endDate.value;
            if (els.chargeAccountCount) els.chargeAccountCount.textContent = resp.accountCount || accounts.length;

            // Store temp file path for later use
            if (resp.tempFilePath) {
                state.pendingTempFilePath = resp.tempFilePath;
            }

            if (!resp.hasTransactions) {
                if (els.chargeNote) {
                    els.chargeNote.textContent = 'Note: No transactions were found for the selected period. You will still be charged the minimum fee.';
                    els.chargeNote.style.color = '#D97706';
                }
            } else {
                if (els.chargeNote) {
                    els.chargeNote.textContent = 'Standard statement fees apply per the bank\'s tariff. Do you want to proceed?';
                    els.chargeNote.style.color = '#6B7280';
                }
            }
        } else {
            if (els.chargeAmount) els.chargeAmount.textContent = 'Calculated upon generation';
            if (els.pageCount) els.pageCount.textContent = 'Unknown';
        }
    }).fail(function () {
        if (els.chargeAmount) els.chargeAmount.textContent = 'Will be calculated';
        if (els.pageCount) els.pageCount.textContent = 'Estimated';
    });
}

// Cancel button - cleanup temp file
if (els.cancelCharge) {
    els.cancelCharge.addEventListener('click', function () {
        // Clean up temp file if it exists
        if (state.pendingTempFilePath) {
            $.ajax({
                url: '/Home/CleanupTempFile',
                method: 'POST',
                data: { filePath: state.pendingTempFilePath },
                complete: function () {
                    state.pendingTempFilePath = null;
                }
            });
        }
        hideModal(els.chargeModal);
        state.pending = { accounts: [], fileNamePrefix: '' };
    });
}

// Confirm button - generate or use existing PDF
if (els.confirmCharge) {
    els.confirmCharge.addEventListener('click', function () {
        if (!state.pending.accounts.length) { hideModal(els.chargeModal); return; }
        generatePdf(state.pending.accounts, state.pending.fileNamePrefix);
    });
}

// Generate PDF with temp file support
function generatePdf(accounts, fileNamePrefix) {
    setBusy(els.confirmCharge, true, 'Generating...');
    var payload = buildPdfRequest(accounts);

    var data = {
        requestJson: JSON.stringify(payload),
        isAjax: true
    };

    // If we have a temp file from preview, use it
    if (state.pendingTempFilePath) {
        data.tempFilePath = state.pendingTempFilePath;
    }

    $.ajax({
        url: '/Home/GeneratePdfFromData',
        method: 'POST',
        contentType: 'application/x-www-form-urlencoded',
        data: data,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'blob';
            return xhr;
        }
    }).done(function (data, status, xhr) {
        var contentType = xhr.getResponseHeader('Content-Type') || '';

        if (contentType.indexOf('application/pdf') !== -1) {
            var chargeAmount = xhr.getResponseHeader('X-Pdf-Amount') || '0.00';
            var pageCount = xhr.getResponseHeader('X-Pdf-Pages') || '0';
            var message = xhr.getResponseHeader('X-Pdf-Message') || 'Statement generated successfully';

            var blob = new Blob([data], { type: 'application/pdf' });
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = fileNamePrefix + '_' + (payload.CIF_ID || 'statement') + '_' +
                String(payload.EndDate || '').replace(/-/g, '') + '.pdf';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);

            hideModal(els.chargeModal);

            var successMsg = 'Your statement PDF has been downloaded successfully.';
            if (parseFloat(chargeAmount) > 0) {
                successMsg += '\n\nCharge: ' + chargeAmount + ' cedis for ' + pageCount + ' page(s).';
            }
            showSuccess(successMsg);

            // Reset temp file
            state.pendingTempFilePath = null;
        } else {
            var reader = new FileReader();
            reader.onload = function () {
                var msg = 'No transactions exist for the selected accounts.';
                try {
                    var jsonResponse = JSON.parse(reader.result);
                    msg = jsonResponse.message || msg;
                } catch (_) { }
                hideModal(els.chargeModal);
                showError(msg);
                state.pendingTempFilePath = null;
            };
            reader.readAsText(data);
        }
    }).fail(function (xhr, status, error) {
        hideModal(els.chargeModal);
        showError('An error occurred while generating the PDF. Please try again.');
        console.error('PDF Generation Error:', error);
        state.pendingTempFilePath = null;
    }).always(function () {
        setBusy(els.confirmCharge, false);
        state.pending = { accounts: [], fileNamePrefix: '' };
    });
}

// ---------------------------------------------------------------
// EXCEL MODAL
// ---------------------------------------------------------------
if (els.downloadExcelBtn) {
    els.downloadExcelBtn.addEventListener('click', function () {
        if (!state.customerAccounts.length) { showError('Please search for a customer first.'); return; }
        if (!els.excelModal) { showError('Excel export is not available on this page yet.'); return; }
        renderSelectionRows(els.excelTableBody, 'excel-cb', { showBalance: true });
        wireSelectionControls(els.selectAllExcel, 'excel-cb', els.downloadSelectedExcel);
        showModal(els.excelModal);
    });
}

if (els.downloadSelectedExcel) {
    els.downloadSelectedExcel.addEventListener('click', function () {
        var sel = getSelected('excel-cb');
        if (!sel.length) { showError('Select at least one account.'); return; }
        downloadExcel(sel);
    });
}

if (els.downloadAllExcel) {
    els.downloadAllExcel.addEventListener('click', function () {
        downloadExcel(state.customerAccounts);
    });
}

function downloadExcel(accounts) {
    var first = accounts && accounts.length ? normalizeAccount(accounts[0]) : {};
    var payload = {
        StartDate: els.startDate.value,
        EndDate: els.endDate.value,
        CIF_ID: first.CIF_ID || '',
        AccountsJson: JSON.stringify(accounts.map(buildAccountPayload))
    };

    var form = document.createElement('form');
    form.method = 'POST';
    form.action = '/Home/GenerateExcelFromData';
    form.style.display = 'none';

    function addField(name, value) {
        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = name;
        input.value = value;
        form.appendChild(input);
    }
    addField('StartDate', payload.StartDate);
    addField('EndDate', payload.EndDate);
    if (payload.CIF_ID) addField('CIF_ID', payload.CIF_ID);
    addField('AccountsJson', payload.AccountsJson);

    var token = document.querySelector('input[name="__RequestVerificationToken"]');
    if (token) addField('__RequestVerificationToken', token.value);

    document.body.appendChild(form);
    form.submit();

    hideModal(els.excelModal);
    setTimeout(function () {
        if (document.body.contains(form)) document.body.removeChild(form);
    }, 1000);
}

// ---------------------------------------------------------------
// EMAIL MODAL
// ---------------------------------------------------------------
if (els.emailBtn) {
    els.emailBtn.addEventListener('click', function () {
        if (!state.customerAccounts.length) {
            showError('Please search for a customer first.');
            return;
        }
        renderSelectionRows(els.emailTableBody, 'email-cb', { showBalance: false });
        wireSelectionControls(els.selectAllEmail, 'email-cb', els.emailSelectedBtn);
        showModal(els.emailModal);
    });
}

if (els.emailSelectedBtn) {
    els.emailSelectedBtn.addEventListener('click', function () {
        var sel = getSelected('email-cb');
        if (!sel.length) { showError('Select at least one account.'); return; }
        sendEmail(sel);
    });
}

if (els.emailAllBtn) {
    els.emailAllBtn.addEventListener('click', function () {
        sendEmail(state.customerAccounts);
    });
}

function sendEmail(accounts) {
    var first = accounts && accounts.length ? normalizeAccount(accounts[0]) : {};
    var recipientEmail = first.Email || '';
    if (!recipientEmail || recipientEmail === '\u2014' || recipientEmail === '—') {
        showError('No valid email address was found for these accounts. Please update the customer\'s email information.');
        return;
    }

    setBusy(els.emailSelectedBtn, true, 'Sending...');
    setBusy(els.emailAllBtn, true, 'Sending...');

    var formData = new FormData();
    formData.append('StartDate', els.startDate.value);
    formData.append('EndDate', els.endDate.value);
    if (first.CIF_ID) formData.append('CIF_ID', first.CIF_ID);
    formData.append('Email', recipientEmail);
    formData.append('AccountsJson', JSON.stringify(accounts.map(buildAccountPayload)));

    var token = document.querySelector('input[name="__RequestVerificationToken"]');
    if (token) formData.append('__RequestVerificationToken', token.value);

    fetch('/Home/EmailStatement', {
        method: 'POST',
        body: formData,
        headers: {
            'Accept': 'application/json'
        }
    })
        .then(function (response) {
            if (!response.ok) throw new Error('HTTP ' + response.status);
            return response.json();
        })
        .then(function (data) {
            if (data && data.success) {
                hideModal(els.emailModal);
                showSuccess(data.message || 'Statement emailed successfully.');
            } else {
                throw new Error((data && data.message) || 'Failed to send email.');
            }
        })
        .catch(function (err) {
            showError('Error: ' + err.message);
        })
        .finally(function () {
            setBusy(els.emailSelectedBtn, false);
            setBusy(els.emailAllBtn, false);
        });
}
        }
    }) ();