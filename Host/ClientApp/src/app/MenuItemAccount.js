import React from 'react';
import { NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { useReactOidc } from "@axa-fr/react-oidc-context";
import { Locale } from "../translations/locale";
import { locales } from "./locales";

export const MenuItemAccount = () => {
    const { oidcUser, logout } = useReactOidc();

    const signOut = (e) => {
        e.preventDefault();
        logout();
    };

    return (
        <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>
                <span>{oidcUser.profile.name}</span>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem>
                    <NavLink to="/" onClick={signOut}>
                        <Locale id={"header-admin-logout"} locales={locales}>Log out</Locale>
                    </NavLink>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    );
}
