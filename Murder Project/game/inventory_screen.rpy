screen inv_hud():
    modal False

    imagebutton auto "bg_inventory_%s.png":
        focus_mask True
        hovered SetVariable("screen_tooltip", "Inventory")
        unhovered SetVariable("screen_tooltip", "")
        action Show("inventory"), Hide("inv_hud")

screen inventory():
    add "bg_inventory_screen"
    modal True

    vbox:
        pos 0.1, 0.25
        for elem in inv.get_items():
            text "[elem.name] - [elem.description]\n"

    imagebutton auto "inventory_goback_%s.png":
        focus_mask True
        hovered SetVariable("screen_tooltip", "Go back")
        unhovered SetVariable("screen_tooltip", "")
        action Hide("inventory"), Show("inv_hud")