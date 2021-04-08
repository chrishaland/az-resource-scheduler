import React from 'react';
import { Table, Button } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import './styles.css';

export const EnvironmentList = (props) => {
    const { environments, onSelectEnvironment } = props;

    return (
        <Table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>
                        <Locale id={"list-header-name"} locales={locales}>Name</Locale>
                    </th>
                    <th>
                        <Locale id={"list-header-description"} locales={locales}>Description</Locale>
                    </th>
                    <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
                {environments.sort((a, b) => (a.description > b.description) ? 1 : ((b.description > a.description) ? -1 : 0)).map((environment, index) => (
                    <tr key={index + 1}>
                        <th scope="row">{index + 1}</th>
                        <td>{environment.name}</td>
                        <td>{environment.description}</td>
                        <td className="right-align">
                            <Button color="info" onClick={() => onSelectEnvironment(environment.id)}>
                                <Locale id={"list-header-edit"} locales={locales}>Edit</Locale>
                            </Button>
                        </td>
                    </tr>
                ))}
                <tr key="add">
                    <th>&nbsp;</th>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td className="right-align">
                        <Button color="info" onClick={() => onSelectEnvironment("")}>
                            <Locale id={"add"} locales={locales}>Add</Locale>
                        </Button>
                    </td>
                </tr>
            </tbody>
        </Table>
    );
};
