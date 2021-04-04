import React from 'react';
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { useLanguage } from "../translations/store";
import { Resource } from "../translations/resource";
import { resources } from "./resources";

export const MenuItemLanguage = () => {
    const { state, dispatch } = useLanguage();

    const changeLanguage = language => {
        localStorage.setItem("language", language);
        dispatch({ type: "changeLanguage", language: language });
    };

    return (
        <UncontrolledDropdown>
            <DropdownToggle nav caret>
                <Resource id={"language-header"} resources={resources}>Language</Resource>
            </DropdownToggle>
            <DropdownMenu>
                <DropdownItem disabled={state.language === "no"} onClick={() => changeLanguage("no")}>
                    <Resource id={"language-no"} resources={resources}>Norwegian</Resource>
                </DropdownItem>
                <DropdownItem disabled={state.language === "en"} onClick={() => changeLanguage("en")}>
                    <Resource id={"language-en"} resources={resources}>English</Resource>
                </DropdownItem>
            </DropdownMenu>
        </UncontrolledDropdown>
    );
}
