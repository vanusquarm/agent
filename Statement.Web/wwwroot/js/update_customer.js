$(document).ready(function () {
    // Email validation
    function validateEmails(emailString) {
        if (!emailString) return true;
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var emails = emailString.split(';').map(function (e) { return e.trim(); }).filter(function (e) { return e; });
        return emails.every(function (email) { return emailRegex.test(email); });
    }

    // Search customer
    $('#searchCustomerBtn').on('click', function (e) {
        e.preventDefault();
        var CIF_ID = $('#searchCustomerNumber').val().trim();
        var $errorMessage = $('#error-message');

        if (!CIF_ID) {
            showError('Please enter a customer CIF ID');
            return;
        }

        var $button = $(this);
        var $buttonText = $button.find('.button-text');
        var $buttonLoader = $button.find('.button-loader');

        $button.prop('disabled', true);
        $buttonText.hide();
        $buttonLoader.show();
        hideError();

        var url = window.profileUrls?.updateCustomer || '/Profile/UpdateCustomer';

        $.ajax({
            url: url,
            type: 'GET',
            data: { CIF_ID: CIF_ID },
            success: function (response) {
                // Check if response contains the form section
                var $newFormSection = $(response).find('#formSection');
                if ($newFormSection.length) {
                    // Replace the entire form section
                    $('#formSection').replaceWith($newFormSection);
                    // Show the form section
                    $('#formSection').show();
                    // Bind events for the new form
                    bindFormEvents();

                    // Check for any error messages in the response
                    var $error = $(response).find('#error-message');
                    if ($error.length && $error.html().trim()) {
                        showError($error.text());
                    }
                } else {
                    // Check if the response itself is an error
                    var $error = $(response).find('#error-message');
                    if ($error.length && $error.html().trim()) {
                        showError($error.text());
                    } else {
                        showError('Customer not found');
                    }
                }
            },
            error: function (xhr) {
                console.error('Search error:', xhr.responseText);
                var message = xhr.responseJSON?.message || xhr.statusText || 'Please try again';
                showError('Error searching customer: ' + message);
            },
            complete: function () {
                $button.prop('disabled', false);
                $buttonText.show();
                $buttonLoader.hide();
            }
        });
    });

    function bindFormEvents() {
        $('#updateCustomerForm').off('submit').on('submit', function (e) {
            e.preventDefault();

            var $form = $(this);
            var $button = $form.find('button[type="submit"]');
            var $buttonText = $button.find('.button-text');
            var $buttonLoader = $button.find('.button-loader');

            // Validate emails
            var emailInput = $('#email').val().trim();
            if (!validateEmails(emailInput)) {
                showError('Please enter valid email addresses separated by semicolons');
                return;
            }

            // Set lastupdatedby from current user
            var userName = '@User.Identity?.Name' || 'System';
            $('#lastupdatedby').val(userName);

            $button.prop('disabled', true);
            $buttonText.hide();
            $buttonLoader.show();
            hideError();

            // Update Continuous field
            $form.find('input[name="Continuous"]').val($('#continuous').is(':checked').toString().toLowerCase());

            // Handle unchecked checkboxes
            ['Frequency.Daily', 'Frequency.Weekly', 'Frequency.Monthly'].forEach(function (name) {
                var $cb = $form.find('input[type="checkbox"][name="' + name + '"]');
                if (!$cb.is(':checked')) {
                    $form.append('<input type="hidden" class="temp-unchecked" name="' + name + '" value="false" />');
                }
            });

            var formData = $form.serialize();
            $form.find('.temp-unchecked').remove();

            var url = window.profileUrls?.updateCustomerPost || '/Profile/UpdateCustomer';

            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: function (response) {
                    if (response.success) {
                        $('#modalMessage').text(response.message || 'Customer updated successfully');
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
                        showError(response.message || 'Failed to update customer');
                    }
                },
                error: function (xhr) {
                    console.error('Submit error:', xhr.responseText);
                    var message = xhr.responseJSON?.message || xhr.statusText || 'Please try again';
                    showError('Error updating customer: ' + message);
                },
                complete: function () {
                    $button.prop('disabled', false);
                    $buttonText.show();
                    $buttonLoader.hide();
                }
            });
        });

        // Email validation on input
        $('#email').on('input', function () {
            var emailInput = $(this).val().trim();
            if (emailInput && !validateEmails(emailInput)) {
                showError('Please enter valid email addresses separated by semicolons');
            } else {
                hideError();
            }
        });
    }

    function showError(message) {
        $('#error-message').removeClass('hidden').find('#error-text').html(message);
    }

    function hideError() {
        $('#error-message').addClass('hidden');
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

    // Show success message if present
    var successMessage = $('.success-message');
    if (successMessage.length && successMessage.text().trim() !== '') {
        $('#modalMessage').text(successMessage.text().trim());
        $('#successModal').css('display', 'flex');
    }
});
