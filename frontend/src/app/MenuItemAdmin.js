import React from 'react';
import { NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import { Locale } from "../translations/locale";
import { locales } from "./locales";
import { useAccount } from "../accounts/hooks";

export const MenuItemAdmin = () => {
    const [account] = useAccount();

    return !!account.roles && account.roles.includes('admin') ? (
        <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>
                <Locale id={"header-admin-title"} locales={locales}>Admin</Locale>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/environments">
                            <Locale id={"header-admin-environments"} locales={locales}>Environments</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/resources">
                            <Locale id={"header-admin-resources"} locales={locales}>Resources</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem divider />
                <DropdownItem>
                    <NavItem>
                        <NavLink href="/jobs">
                            <Locale id={"header-admin-jobs"} locales={locales}>Jobs</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem>
                    <NavItem>
                        <NavLink href="/docs">
                            <Locale id={"header-admin-docs"} locales={locales}>Documentation</Locale>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    ) : null;
}
