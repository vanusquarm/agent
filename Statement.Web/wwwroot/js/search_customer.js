
$(document).ready(function () {
    // Search form - show loading
    $('#searchForm').on('submit', function (e) {
        var $btn = $('#searchBtn');
        var $text = $('#btn-text');
        var $loader = $('#btn-loader');

        $text.addClass('hidden');
        $loader.removeClass('hidden');
        $btn.prop('disabled', true);
    });

    // View customer details
    window.viewCustomerDetails = function (cifId) {
        $('#customerDetailsContent').html(`
                    <div class="text-center py-8">
                        <i class="fas fa-spinner fa-spin text-3xl text-orange-500"></i>
                        <p class="text-gray-500 mt-2">Loading customer details...</p>
                    </div>
                `);
        $('#customerDetailsModal').css('display', 'flex');

        $.ajax({
            url: '@Url.Action("GetCustomerDetails", "Profile")',
            type: 'GET',
            data: { cifId: cifId },
            success: function (response) {
                if (response.success) {
                    var data = response.data;
                    var html = `
    <div class="grid grid-cols-2 gap-4">
        <div>
            <label class="text-xs text-gray-500 font-medium">CIF ID</label>
            <p class="font-mono text-sm font-semibold text-orange-600">${data.cifId}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Customer Number</label>
            <p class="font-medium text-gray-800">${data.customerNumber || '—'}</p>
        </div>
        <div class="col-span-2">
            <label class="text-xs text-gray-500 font-medium">Customer Name</label>
            <p class="font-medium text-gray-800">${data.displayName}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Email</label>
            <p class="text-gray-700">${data.email || '—'}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Branch</label>
            <p class="text-gray-700">${data.branchCode || '—'}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Status</label>
            <p class="text-gray-700">${data.status === 'Yes' || data.status === 'Active' ?
                            '<span class="text-green-600 font-medium">Active</span>' :
                            '<span class="text-red-600 font-medium">Inactive</span>'}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Output Format</label>
            <p class="text-gray-700">${data.defaultExt || '—'}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Accounts</label>
            <p class="text-gray-700">${data.accountCount || 0}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">All Accounts</label>
            <p class="text-gray-700">${data.allAccounts ? 'Yes' : 'No'}</p>
        </div>
        <div>
            <label class="text-xs text-gray-500 font-medium">Use Export File</label>
            <p class="text-gray-700">${data.useExportFile ? 'Yes' : 'No'}</p>
        </div>
        <div class="col-span-2 border-t pt-4">
            <h4 class="font-semibold text-gray-700 mb-2">Statement Frequency</h4>
            <div class="grid grid-cols-3 gap-4">
                <div>
                    <label class="text-xs text-gray-500">Daily</label>
                    <p class="text-gray-700">${data.daily ? 'Yes' : 'No'}</p>
                    ${data.dailyOpt ? `<p class="text-xs text-gray-400">Option: ${data.dailyOpt}</p>` : ''}
                </div>
                <div>
                    <label class="text-xs text-gray-500">Weekly</label>
                    <p class="text-gray-700">${data.weekly ? 'Yes' : 'No'}</p>
                    ${data.weeklyOpt ? `<p class="text-xs text-gray-400">Option: ${data.weeklyOpt}</p>` : ''}
                </div>
                <div>
                    <label class="text-xs text-gray-500">Monthly</label>
                    <p class="text-gray-700">${data.monthly ? 'Yes' : 'No'}</p>
                    ${data.monthlyOpt ? `<p class="text-xs text-gray-400">Option: ${data.monthlyOpt}</p>` : ''}
                </div>
            </div>
        </div>
        <div class="col-span-2 border-t pt-4">
            <h4 class="font-semibold text-gray-700 mb-2">Audit Information</h4>
            <div class="grid grid-cols-2 gap-4">
                <div>
                    <label class="text-xs text-gray-500">Created By</label>
                    <p class="text-gray-700">${data.createdBy || '—'}</p>
                    <p class="text-xs text-gray-400">${data.dateCreated ? new Date(data.dateCreated).toLocaleDateString() : '—'}</p>
                </div>
                <div>
                    <label class="text-xs text-gray-500">Last Updated By</label>
                    <p class="text-gray-700">${data.lastUpdatedBy || '—'}</p>
                    <p class="text-xs text-gray-400">${data.lastUpdatedDate ? new Date(data.lastUpdatedDate).toLocaleDateString() : '—'}</p>
                </div>
            </div>
        </div>
    </div>
    `;
                    $('#customerDetailsContent').html(html);
                } else {
                    $('#customerDetailsContent').html(`
                                <div class="text-center py-8">
                                    <i class="fas fa-exclamation-circle text-4xl text-red-500 mb-2"></i>
                                    <p class="text-red-600">${response.message || 'Failed to load customer details'}</p>
                                </div>
                            `);
                }
            },
            error: function () {
                $('#customerDetailsContent').html(`
                            <div class="text-center py-8">
                                <i class="fas fa-exclamation-circle text-4xl text-red-500 mb-2"></i>
                                <p class="text-red-600">An error occurred while loading customer details</p>
                            </div>
                        `);
            }
        });
    };

    // Modal close handlers
    $('.close-modal').click(function () {
        $(this).closest('.modal').hide();
    });

    $(document).on('click', function (e) {
        if ($(e.target).is('.modal')) {
            $('.modal').hide();
        }
    });
});
