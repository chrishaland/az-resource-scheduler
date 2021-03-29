import React from 'react';
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { useLanguage } from "../translations/store";
import { Resource } from "../translations/resource";
import { resources } from "./resources";

export const MenuItemLanguage = () => {
    const { state, dispatch } = useLanguage();

    return (
        <UncontrolledDropdown>
            <DropdownToggle nav caret>
                <Resource id={"language-header"} resources={resources}>Language</Resource>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem disabled={state.language === "no"} onClick={() => dispatch({ type: "changeLanguage", language: "no" })}>
                    <Resource id={"language-no"} resources={resources}>Norwegian</Resource>
                </DropdownItem>
                <DropdownItem disabled={state.language === "en"} onClick={() => dispatch({ type: "changeLanguage", language: "en" })}>
                    <Resource id={"language-en"} resources={resources}>English</Resource>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    );
}
