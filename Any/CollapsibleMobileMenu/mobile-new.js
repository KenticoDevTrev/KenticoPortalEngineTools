jQuery(document).ready(function () {
    // Put class name here for each menu item.
    initializeMenu("mobileMenu", true, true, false, true, false);
});

// of mobile, has mobile icon
// if off-click hide

function initializeMenu(containerClassName, hasMenuIcon, collapseMenuOnOffClick, collapseSubMenusOnOffClick, expandedToCurrentPage, expandChildOfCurrent) {
    jQuery("." + containerClassName + " .CollapsibleNavigation").each(function () {
        jQuery(this).hide().removeClass("hidden");
    });

    if (expandedToCurrentPage) {
        // Code to expand from current page up here, including the main nav.
        var selectedPage = jQuery("." + containerClassName + " li.page.current").parents("ul.CollapsibleNavigation");
        while (selectedPage.parents("ul.CollapsibleNavigation").length != 0) {
            selectedPage.prev().addClass("opened");
            selectedPage.removeClass("hidden");
            selectedPage.show();
            selectedPage = selectedPage.parents("ul.CollapsibleNavigation");
        }
    }
  
    if (expandChildOfCurrent) {
        var selectedPage = jQuery("." + containerClassName + " li.page.current");
        jQuery(".pageContainer", selectedPage).first().addClass("opened");
        jQuery("ul", selectedPage).first().show();
      }

    if (hasMenuIcon) {
        // Add click on button
        jQuery("." + containerClassName + " .toggleButton").click(function () {
            toggleMenu(containerClassName, null, collapseSubMenusOnOffClick);
        });
      
        // Close initial menu since expandToCurrentPage expands all the way
        jQuery(jQuery("." + containerClassName + " .CollapsibleNavigation")[0]).hide();
      
        // Add check to close if clicked off
        if (collapseMenuOnOffClick) {
            jQuery(document).click(function (event) {
                if ((jQuery(event.target).parents().index(jQuery('.CollapsibleNavigation')) == -1 && jQuery(event.target).index(jQuery(".CollapsibleNavigation")) == -1) && jQuery(event.target).parents().index(jQuery('.' + containerClassName + ' .toggleButton')) == -1 && jQuery(event.target).index(jQuery('.' + containerClassName + ' .toggleButton')) == -1) {
                    toggleMenu(containerClassName, false, collapseSubMenusOnOffClick);
                }
            });
        }
    } else {
        // ensure main menu is visible
        jQuery(jQuery("." + containerClassName + " ul.CollapsibleNavigation")[0]).removeClass("hidden").show();
    }



    jQuery("." + containerClassName + " .pageContainer .expandable").each(function () {
        var button = jQuery(this);
        if (jQuery(".CollapsibleNavigation", button.parent().parent().parent()).length > 0) {
            button.click(function () {
                toggleSubMenu(button);
            });
        } else {
            button.addClass("hidden");
        }
    });
}

// Send the jQuery of the clicked expandable object
function toggleSubMenu(buttonObject) {
    jQuery(jQuery(".CollapsibleNavigation", buttonObject.parent().parent().parent())[0]).slideToggle(300);
    buttonObject.parent().parent().toggleClass("opened");
}

// Toggles menu, forceToggle can be set to true for force open, false to force close, or not provided to toggle
function toggleMenu(menuClassName, forceToggle, collapseSubMenusOnOffClick) {
    var menuButton = jQuery("." + menuClassName + " .toggleButton");
    if (forceToggle == null) {
        menuButton.toggleClass("opened");
        jQuery(jQuery("." + menuClassName + " .CollapsibleNavigation")[0]).slideToggle(300);
    } else {
        if (forceToggle) {
            // Force Open
            menuButton.addClass("opened");
            jQuery(jQuery("." + menuClassName + " .CollapsibleNavigation")[0]).slideDown(300);
        } else {
            // Force Close
            menuButton.removeClass("opened");
            jQuery(jQuery("." + menuClassName + " .CollapsibleNavigation")[0]).slideUp(300);
        }
    }

    if (collapseSubMenusOnOffClick) {
        if (!menuButton.hasClass("opened")) {
            jQuery("ul", menuButton.parent()).each(function () {
                jQuery(this).slideUp(300);
            });
            jQuery(".opened", menuButton.parent()).each(function () {
                jQuery(this).removeClass("opened");
            });
        }
    }
}