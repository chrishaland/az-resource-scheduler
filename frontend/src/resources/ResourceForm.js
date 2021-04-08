import React from "react";
import { Input, Button, ButtonGroup } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { useResource, useResources } from './hooks';
import './styles.css';

export const ResourceForm = (props) => {
    const { id, onSelectResource, removeSelectedResource } = props;
    const [resource, changeResource, upsert] = useResource(id);

    const handleTypeChange = (vm, vmss) => {
        changeResource("virtualMachineExtentions", vm);
        changeResource("virtualMachineScaleSetExtentions", vmss);
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
                    <Button color="info" type="submit">
                        <Locale id={"form-save"} locales={locales}>Save</Locale>
                    </Button>
                </div>
            </form>

            {false ? (
                <div className="col right-align">
                    <Button color="info" onClick={() => onSelectResource("")}>
                        <Locale id={"add"} locales={locales}>Add</Locale>
                    </Button>
                </div>
            ) : null}
        </>
    );
};
