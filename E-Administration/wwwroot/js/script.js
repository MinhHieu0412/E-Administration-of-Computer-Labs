/*      Custom Script       */ 

$(document).ready(function(){

    // Banner Section Owl Carousel Slider
    $("#banner-section .owl-carousel").owlCarousel({
        dots:false,
        items:1,
        loop:true,
        autoplay:true
    });

     // Lab Section Owl Carousel Slider
    $("#lab-section .owl-carousel").owlCarousel({
        dots:false,
        loop:true,
        autoplay:true,
        responsive:{
            0:{
                items:2
            },
            600:{
                items:3
            },
            1000:{
                items:4
            }
        }
    });

     // Course Section Owl Carousel Slider
    $("#course-section .owl-carousel").owlCarousel({
        dots:false,
        loop:true,
        autoplay:true,
        responsive:{
            0:{
                items:2
            },
            600:{
                items:3
            },
            1000:{
                items:4
            }
        }
    });

     // Instructor Owl Carousel Slider
    $("#instructor-section .owl-carousel").owlCarousel({
        dots:false,
        loop:true,
        responsive:{
            0:{
                items:2
            },
            600:{
                items:3
            },
            1000:{
                items:4
            }
        }
    });

     // Management Staff Owl Carousel Slider
    $("#management-section .owl-carousel").owlCarousel({
        dots:false,
        loop:true,
        responsive:{
            0:{
                items:2
            },
            600:{
                items:3
            },
            1000:{
                items:4
            }
        }
    });

    // Disabling to Drag Image from HTML Page
    $('img').on('dragstart', function(event) { event.preventDefault(); });

    $('#dashboard-section .toggler-btn').click(function(){
        $('#side-Nav').slideToggle();
    });

});



function openPage(pageName, elmnt, color) {
    // Remove the background color of all tablinks/buttons
    var tablinks = document.getElementsByClassName("tablink");
    for (var i = 0; i < tablinks.length; i++) {
        tablinks[i].style.backgroundColor = ""; // Reset background color
        tablinks[i].style.color = ""; // Reset text color if needed
    }

    // Add the specific color to the button used to open the tab content
    if (elmnt) {
        elmnt.style.backgroundColor = color;
        elmnt.style.color = "white"; // Optional: make text color white for contrast
    }

    // Redirect to the corresponding page (if necessary)
    const pageUrls = {
        Administrators: "/Admin/Administrators",
        Lecturer: "/Admin/Lecturer",
        Staff: "/Admin/Staff",
        Students: "/Admin/Accounts/Students",
    };

    if (pageUrls[pageName]) {
        window.location.href = pageUrls[pageName];
    } else {
        console.error("Invalid page name: " + pageName);
    }
}

// Example usage
document.getElementById("defaultOpen").click();
