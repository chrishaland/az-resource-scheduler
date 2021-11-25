import React from 'react';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import { Layout } from './app/Layout';
import { Scheduler } from './scheduler/Scheduler';
import { Resources } from './resources/Resources';
import { Providers } from './providers/Providers';
import { Environments } from './environments/Environments';
import { LanguageProvider } from './translations/store';
import { FeatureFlagsProvider } from './feature_flags/store';
import { EnvironmentsProvider } from './environments/store';
import { OidcProvider } from "./oidc/store";

export const App = () => {
    return (
        <LanguageProvider>
            <OidcProvider>
                <FeatureFlagsProvider>
                    <EnvironmentsProvider>
                        <BrowserRouter>
                            <Layout>
                                <Switch>
                                    <Route exact path='/' component={Scheduler} />
                                    <Route exact path='/resources' component={Resources} />
                                    <Route exact path='/providers' component={Providers} />
                                    <Route exact path='/environments' component={Environments} />
                                    <Route component={() => (<div>404 - Not Found</div>)} />
                                </Switch>
                            </Layout>
                        </BrowserRouter>
                    </EnvironmentsProvider>
                </FeatureFlagsProvider>
            </OidcProvider>
        </LanguageProvider>
    );
}

export default App;
