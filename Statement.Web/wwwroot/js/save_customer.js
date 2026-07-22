
$(document).ready(function () {
    // Search customer
    $('#searchCustomerBtn').on('click', function (e) {
        e.preventDefault();
        var customerNumber = $('#searchCustomerNumber').val().trim();

        var url = window.profileUrls?.newCustomer || '/Profile/NewCustomer';

        if (!customerNumber) {
            showError('Please enter a customer account number');
            return;
        }

        var $button = $(this);
        var $buttonText = $button.find('.button-text');
        var $buttonLoader = $button.find('.button-loader');

        $button.prop('disabled', true);
        $buttonText.hide();
        $buttonLoader.show();
        hideError();

        $.ajax({
            url: url,
            type: 'GET',
            data: { accountNumber: customerNumber },
            success: function (response) {
                // Update hidden fields
                var $response = $(response);

                // Update hidden inputs
                $('#customerNumber').val($response.find('#customerNumber').val());
                $('#cif_id').val($response.find('#cif_id').val());
                $('#customerName').val($response.find('#customerName').val());
                $('#branch').val($response.find('#branch').val());

                // Update display fields
                $('#branchDisplay').val($response.find('#branchDisplay').val());
                $('#cifDisplay').val($response.find('#cifDisplay').val());
                $('#customerNameDisplay').val($response.find('#customerNameDisplay').val());
                $('#email').val($response.find('#email').val());

                // Update accounts dropdown
                var accountsHtml = $response.find('#accounts').html();
                if (accountsHtml) {
                    $('#accounts').html(accountsHtml);
                }

                // Check for errors
                var error = $response.find('#error-message .text-red-700').text();
                if (error && error.trim()) {
                    showError(error);
                }
            },
            error: function (xhr) {
                console.error('Search error:', xhr.responseText);
                showError('Error searching customer: ' + (xhr.statusText || 'Please try again'));
            },
            complete: function () {
                $button.prop('disabled', false);
                $buttonText.show();
                $buttonLoader.hide();
            }
        });
    });

    // Form submit
    $('#newCustomerForm').on('submit', function (e) {
        e.preventDefault();

        var $form = $(this);
        var $button = $form.find('button[type="submit"]');
        var $buttonText = $button.find('.button-text');
        var $buttonLoader = $button.find('.button-loader');

        // Validate required fields
        var cifId = $('#cif_id').val();
        if (!cifId) {
            showError('Please search for a customer first');
            return;
        }

        // Set Createdby from current user
        var userName = '@User.Identity?.Name' || 'System';
        $('#createdby').val(userName);

        // Ensure UseExportFile is properly set
        var useExportFile = $('#useExportFile').is(':checked');
        $('input[name="UseExportFile"]').val(useExportFile);

        // Ensure Allaccounts is properly set
        var allAccounts = $('#allAccounts').is(':checked');
        $('input[name="Allaccounts"]').val(allAccounts);

        // Show loading
        $button.prop('disabled', true);
        $buttonText.hide();
        $buttonLoader.show();
        hideError();

        var formData = $form.serialize();

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: formData,
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (response) {
                if (response.success) {
                    $('#modalMessage').text(response.message || 'Customer saved successfully');
                    $('#successModal').css('display', 'flex');
                    setTimeout(function () {
                        location.reload();
                    }, 3000);
                } else if (response.errors) {
                    var errorHtml = response.errors.map(function (err) {
                        return '<p>' + err + '</p>';
                    }).join('');
                    showError(errorHtml);
                } else {
                    showError(response.message || 'Failed to save customer');
                }
            },
            error: function (xhr) {
                console.error('Submit error:', xhr.responseText);
                showError('Error saving customer: ' + (xhr.statusText || 'Please try again'));
            },
            complete: function () {
                $button.prop('disabled', false);
                $buttonText.show();
                $buttonLoader.hide();
            }
        });
    });

    // Modal close handlers
    $('.close-modal').click(function () {
        $(this).closest('.modal').hide();
    });

    $(document).on('click', function (e) {
        if ($(e.target).is('.modal')) {
            $('.modal').hide();
        }
    });

    function showError(message) {
        $('#error-message').removeClass('hidden').find('#error-text').html(message);
    }

    function hideError() {
        $('#error-message').addClass('hidden');
    }
});