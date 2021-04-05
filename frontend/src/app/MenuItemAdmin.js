import React from 'react';
import { NavItem, NavLink, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import { Resource } from "../translations/resource";
import { resources } from "./resources";
import { useAccount } from "../accounts/hooks";

export const MenuItemAdmin = () => {
    const [account] = useAccount();

    return !!account.roles && account.roles.includes('admin') ? (
        <UncontrolledDropdown nav inNavbar>
            <DropdownToggle nav caret>
                <Resource id={"header-admin-title"} resources={resources}>Admin</Resource>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/tenants">
                            <Resource id={"header-admin-tenants"} resources={resources}>Tenants</Resource>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem>
                    <NavItem>
                        <NavLink tag={Link} to="/environments">
                            <Resource id={"header-admin-environments"} resources={resources}>Environments</Resource>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
                <DropdownItem divider />
                <DropdownItem>
                    <NavItem>
                        <NavLink href="/jobs">
                            <Resource id={"header-admin-jobs"} resources={resources}>Jobs</Resource>
                        </NavLink>
                    </NavItem>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    ) : null;
}
