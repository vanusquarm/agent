
$(document).ready(function () {
    // Search customer
    $('#searchCustomerBtn').on('click', function (e) {
        e.preventDefault();
        var customerNumber = $('#searchCustomerNumber').val().trim();
        if (!customerNumber) {
            showError('Please enter a customer account number');
            return;
        }

        var $button = $(this);
        var $buttonText = $button.find('.button-text');
        var $buttonLoader = $button.find('.button-loader');

        // Show loading state
        $button.prop('disabled', true);
        $buttonText.hide();
        $buttonLoader.show();
        hideError();

        $.ajax({
            url: '@Url.Action("NewCustomer", "Profile")',
            type: 'GET',
            data: { accountNumber: customerNumber },
            success: function (response) {
                var $newForm = $(response).find('#newCustomerForm');
                if ($newForm.length) {
                    $('#newCustomerForm').replaceWith($newForm);
                    $('#newCustomerForm :input').not('[readonly]').removeAttr('disabled');
                    bindFormEvents();
                    showSuccess(response);
                } else {
                    showError('Customer not found');
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

    function bindFormEvents() {
        $('#newCustomerForm').off('submit').on('submit', function (e) {
            e.preventDefault();

            var $form = $(this);
            var $button = $form.find('button[type="submit"]');
            var $buttonText = $button.find('.button-text');
            var $buttonLoader = $button.find('.button-loader');

            // Validate email
            var email = $('#email').val().trim();
            if (email && !isValidEmail(email)) {
                showError('Please enter a valid email address');
                return;
            }

            // Show loading state
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
    }

    function isValidEmail(email) {
        var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    }

    function showError(message) {
        $('#error-message').removeClass('hidden').find('#error-text').text(message);
    }

    function hideError() {
        $('#error-message').addClass('hidden');
    }

    function showSuccess(response) {
        var $success = $(response).find('.bg-green-50');
        if ($success.length) {
            $('.bg-green-50').replaceWith($success);
        }
    }

    // Modal close handlers
    $('.close-modal').click(function () {
        $(this).closest('.modal').hide();
    });

    $(document).on('click', function (e) {
        if ($(e.target).is('.modal')) {
            $('.modal').hide();
        }
    });

    // Initialize form events
    bindFormEvents();
});