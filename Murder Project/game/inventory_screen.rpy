screen inv_hud():
    modal False

    imagebutton auto "bg_inventory_%s.png":
        focus_mask True
        hovered SetVariable("screen_tooltip", "Inventory")
        unhovered SetVariable("screen_tooltip", "")
        action Show("inventory"), Hide("inv_hud")