import React from 'react';
import { NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import { Locale } from "../translations/locale";
import { locales } from "./locales";
import { useAccount } from "../accounts/hooks";

export const MenuItemAccount = () => {
    const [account, refetchAccount] = useAccount();

    return !!account.email ? (
        <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>
                {account.name}
            </DropdownToggle>
            <DropdownMenu>
                {account.featureFlags.includes('profile') ? (
                    <DropdownItem>
                        <NavLink tag={Link} to="/profile">
                            <Locale id={"header-admin-profile"} locales={locales}>Profile</Locale>
                        </NavLink>
                    </DropdownItem>
                ) : null}
                <DropdownItem>
                    <NavLink href="/api/account/logout">
                        <Locale id={"header-admin-logout"} locales={locales}>Log out</Locale>
                    </NavLink>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    ) : (
        <NavItem>
            <NavLink href="/api/account/login">
                <Locale id={"header-admin-login"} locales={locales}>Log in</Locale>
            </NavLink>
        </NavItem>
    );
}
