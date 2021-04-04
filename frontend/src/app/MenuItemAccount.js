import React from 'react';
import { NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Resource } from "../translations/resource";
import { resources } from "./resources";
import { useAccount } from "../accounts/hooks";

export const MenuItemAccount = () => {
    const [account, refetchAccount] = useAccount();

    return !!account.email ? (
        <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>
                {account.name}
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem>
                    <NavLink href="/api/account/logout">
                        <Resource id={"header-admin-logout"} resources={resources}>Log out</Resource>
                    </NavLink>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    ) : (
        <NavItem>
            <NavLink href="/api/account/login">
                <Resource id={"header-admin-login"} resources={resources}>Log in</Resource>
            </NavLink>
        </NavItem>
    );
}
