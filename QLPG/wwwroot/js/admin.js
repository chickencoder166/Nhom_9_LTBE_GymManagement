// Admin JavaScript

$(document).ready(function() {
    console.log('Admin layout loaded');
    
    // Sidebar toggle for mobile
    $('#sidebarToggle').on('click', function() {
        $('#sidebar').toggleClass('show');
    });
    
    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
    
    // Confirm before delete
    $('.btn-delete').on('click', function(e) {
        if (!confirm('B?n có ch?c ch?n mu?n xóa?')) {
            e.preventDefault();
        }
    });
});

// Delete student function (global)
function showDeleteModal(maSV, hoTen) {
    $('#deleteStudentId').val(maSV);
    $('#deleteStudentName').text(hoTen);
    $('#deleteModal').modal('show');
}
