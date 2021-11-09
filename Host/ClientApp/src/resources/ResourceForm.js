import React, { useState } from "react";
import { Input, InputGroup, InputGroupAddon, InputGroupText, InputGroupButtonDropdown, Button, ButtonGroup, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { useProviders } from '../providers/hooks';
import { useEnvironments } from '../environments/hooks';
import { useResource } from './hooks';
import './styles.css';

export const ResourceForm = (props) => {
    const { id, removeSelectedResource } = props;
    const [resource, changeResource, upsert] = useResource(id);

    const [dropdownOpen, setDropdownOpen] = useState(false);
    const toggleDropDown = () => setDropdownOpen(!dropdownOpen);

    const { providers } = useProviders();
    const { environments } = useEnvironments();

    const handleTypeChange = (vm, vmss) => {
        changeResource("virtualMachineExtentions", vm);
        changeResource("virtualMachineScaleSetExtentions", vmss);
    };

    const handleEnvironmentsChange = (event) => {
        const environmentId = event.target.value;
        const environmentIds = resource.environmentIds;
        if (event.target.checked) {
            if (environmentIds.includes(environmentId)) return;
            environmentIds.push(environmentId);
            changeResource("environmentIds", environmentIds);
        }
        else {
            if (!environmentIds.includes(environmentId)) return;
            environmentIds.splice(environmentIds.indexOf(environmentId), 1);
            changeResource("environmentIds", environmentIds);
        }
    };

    const handleChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        changeResource(name, value);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        upsert(removeSelectedResource);
    };

    return (
        <>
            <div className="col right-align">
                <Button onClick={removeSelectedResource} outline color="danger" type="submit">
                    <Locale id={"form-close"} locales={locales}>Close</Locale>
                </Button>
            </div>
            {!!!resource.id ? (
                <>
                    <div className="col">
                        <Locale id={"form-type"} locales={locales}>Resource type</Locale>
                    </div>
                    <div className="col">
                        <ButtonGroup>
                            <Button color="info" onClick={() => handleTypeChange({}, null)}>
                                <Locale id={"form-type-vm"} locales={locales}>Virtual Machine</Locale>
                            </Button>
                            <Button color="info" onClick={() => handleTypeChange(null, { capacity: 0 })}>
                                <Locale id={"form-type-vmss"} locales={locales}>Virtual Machine Scale Set</Locale>
                            </Button>
                        </ButtonGroup>
                    </div>
                </>
            ) : null}
            <form onSubmit={handleSubmit}>
                <div className="col">
                    <Locale id={"form-id"} locales={locales}>Id</Locale>
                    <Input type="text" name="id" value={resource.id} disabled />
                </div>
                <div className="col">
                    <Locale id={"form-name"} locales={locales}>Name</Locale>
                    <Input type="text" name="name" value={resource.name} onChange={handleChange} />
                </div>
                <div className="col">
                    <Locale id={"form-resourceGroup"} locales={locales}>Resource group</Locale>
                    <Input type="text" name="resourceGroup" value={resource.resourceGroup} onChange={handleChange} />
                </div>
                <div className="col">
                    <Locale id={"form-description"} locales={locales}>Description</Locale>
                    <Input type="text" name="description" value={resource.description} onChange={handleChange} />
                </div>
                {!!resource.virtualMachineScaleSetExtentions ? (
                    <div className="col">
                        <Locale id={"form-capacity"} locales={locales}>Capacity</Locale>
                        <Input type="text" name="virtualMachineScaleSetExtentions.capacity" value={resource.virtualMachineScaleSetExtentions.capacity} onChange={handleChange} />
                    </div>
                ) : null}
                <div className="col">
                    <Locale id={"form-provider"} locales={locales}>Cloud provider</Locale>
                </div>
                <div className="col">
                    <InputGroupAddon addonType="append">
                        <InputGroupButtonDropdown addonType="append" isOpen={dropdownOpen} toggle={toggleDropDown}>
                            <DropdownToggle split outline color="info">
                                <Locale id={providers.find(p => p.id === resource.providerId)?.name ?? "form-provider-id"} locales={locales}>
                                    {providers.find(p => p.id === resource.providerId)?.name ?? "Choose..."}
                                </Locale>
                            </DropdownToggle>
                            <DropdownMenu>
                                {providers.sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0)).map(provider => (
                                    <DropdownItem key={provider.id} onClick={() => changeResource("providerId", provider.id)}>
                                        <Locale id={provider.id} locales={locales}>{provider.name}</Locale>
                                    </DropdownItem>
                                ))}
                            </DropdownMenu>
                        </InputGroupButtonDropdown>
                    </InputGroupAddon>
                </div>
                <div className="col">
                    <Locale id={"form-environments"} locales={locales}>Linked environments</Locale>
                </div>
                {environments.sort((a, b) => (a.description > b.description) ? 1 : ((b.description > a.description) ? -1 : 0)).map((environment, index) => (
                    <div key={index} className="col col-lg-6 environments">
                        <InputGroup>
                            <InputGroupAddon addonType="prepend">
                                <InputGroupText>
                                    <Input addon type="checkbox" checked={resource.environmentIds.includes(environment.id)} value={environment.id} onChange={handleEnvironmentsChange} />
                                </InputGroupText>
                            </InputGroupAddon>
                            <Input placeholder={environment.description || environment.name} disabled />
                        </InputGroup>
                    </div>
                ))}
                <div className="col save">
                    <Button color="info" type="submit">
                        <Locale id={"form-save"} locales={locales}>Save</Locale>
                    </Button>
                </div>
            </form>
        </>
    );
};
