using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NLog;

namespace BEntities
{
	/// <summary>
	/// Entity Component System manager class which is resposible for Entity creation, Systems registering and processing
	/// Incapsulates all interaction with internal ECS Service
	/// </summary>
    public class ECSManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly ECSService _service = new ECSService();
        
		internal ECSManager()
		{

		}
		
		/// <summary>
        /// Tetrieves the system by its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSystem<T>() where T : BaseComponentSystem
        {
            var result = _service.UpdateSystems.FirstOrDefault(e => e.GetType() == typeof(T));
            if (result != null)
                return (T)result;

            result = _service.DrawSystems.FirstOrDefault(e => e.GetType() == typeof(T));
            return (T)result;
        }

        /// <summary>
        /// Registers system by its type and performs all necessary initialization routine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterSystem<T>() where T : BaseComponentSystem
        {
            RegisterSystem(typeof(T));
        }

        /// <summary>
        /// Registers system by its type and performs all necessary initialization routine
        /// </summary>
        public void RegisterSystem(Type type)
        {
            if(!typeof(BaseComponentSystem).IsAssignableFrom(type))
                throw new ArgumentException($"EC System of type {type.Name} is not inherited from {nameof(BaseComponentSystem)}");

			BaseComponentSystem system = (BaseComponentSystem)Activator.CreateInstance(type);
            system.PreInitialize(this);

            if (system.SystemType == SystemProcessingType.Draw)
            {
                _service.DrawSystems.Add(system);
                _service.DrawSystems.Sort((system1, system2) => system1.Order - system2.Order);
                
            }
            else if (system.SystemType == SystemProcessingType.Update)
            {
                _service.UpdateSystems.Add(system);
                _service.UpdateSystems.Sort((system1, system2) => system1.Order - system2.Order);
            }

            system.Initialize();
        }

        /// <summary>
        /// Performs initialization of all scanned systems and templates
        /// </summary>
        internal void Initialize(IEnumerable<Type> availableSystems, IEnumerable<Type> availableTemplates)
        {
            // processing templates
            foreach (Type availableTemplateType in availableTemplates)
            {
                EntityTemplate template = null;
                try
                {
                    template = (EntityTemplate)Activator.CreateInstance(availableTemplateType);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Unable to create template '{availableTemplateType.Name}'");
                    continue;
                }

                template.InitializeInternal();

                _service.Templates[availableTemplateType] = template;
                _service.TemplatesByName[template.Name] = template;
            }

            foreach (Type availableSystem in availableSystems)
            {
                BaseComponentSystem system = null;
                try
                {
                    system = (BaseComponentSystem) Activator.CreateInstance(availableSystem);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Unable to create system '{availableSystem.Name}'");
                    continue;
                }

                system.PreInitialize(this);

                if (system.SystemType == SystemProcessingType.Draw)
                    _service.DrawSystems.Add(system);
                else if (system.SystemType == SystemProcessingType.Update)
                    _service.UpdateSystems.Add(system);
            }

            // performing ordering of systems and initializing
            _service.UpdateSystems.Sort((system1, system2) => system1.Order - system2.Order);
            _service.DrawSystems.Sort((system1, system2) => system1.Order - system2.Order);

            foreach (BaseComponentSystem system in _service.UpdateSystems)
            {
                system.Initialize();
            }

            foreach (BaseComponentSystem system in _service.DrawSystems)
            {
                system.Initialize();
            }
        }

		/// <summary>
		/// Performs the step procedure for Drawning systems
		/// </summary>
		/// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            foreach (BaseComponentSystem baseComponentSystem in _service.DrawSystems)
            {
                baseComponentSystem.Step(gameTime);
            }
        }

		/// <summary>
		/// Performs step procedure for Updating systems
		/// </summary>
		/// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _service.ProcessComponentsForRemoving();
            _service.ProcessEntitiesForRemoving();

            foreach (BaseComponentSystem serviceUpdateSystem in _service.UpdateSystems)
            {
                serviceUpdateSystem.Step(gameTime);
            }

            _service.ProcessEntitiesForAdding();
            _service.ProcessComponentsForRegistering();
        }

		/// <summary>
		/// Performs the immediate destruction of all registered entities
		/// </summary>
        public void DestroyAllEntities()
        {
            _service.DestroyAllEntitiesImmediately();
        }

		// Performs 
        public void UnloadContent()
        {
            DestroyAllEntities();
        }

        public Entity CreateEntity()
        {
            return _service.CreateEntity();
        }

        internal Entity CreateEntityFromTemplateInternal(EntityTemplate template, params object[] args)
        {
            Entity result = _service.CreateEntity();
            template.BuildEntity(result, args);

            return result;
        }

        /// <summary>
        /// Creates entity from '<paramref name="templateType"/>' template
        /// </summary>
        /// <param name="templateType">Type of template</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Entity CreateEntityFromTemplate(Type templateType, params object[] args)
        {
            Entity result = null;

            if (_service.Templates.TryGetValue(templateType, out var template))
            {
                result = CreateEntityFromTemplateInternal(template, args);
            }
            else
            {
                Log.Error($"Unable to build entity from template '{templateType.Name}': this template does not registered in system");
            }

            return result;
        }

        /// <summary>
        /// Creates entity from template '<typeparamref name="T"/>'
        /// </summary>
        /// <typeparam name="T">Template Type</typeparam>
        /// <param name="args">Argumenst which will be passed to template</param>
        /// <returns></returns>
        public Entity CreateEntityFromTemplate<T>(params object[] args) where T : EntityTemplate, new()
        {
            return CreateEntityFromTemplate(typeof(T), args);
        }

        /// <summary>
        /// Creates entuty from template by its name
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Entity CreateEntityFromTemplate(string templateName, params object[] args)
        {
            Entity result = null;

            if (_service.TemplatesByName.TryGetValue(templateName, out var template))
            {
                result = CreateEntityFromTemplateInternal(template, args);
            }
            else
            {
                Log.Error($"Unable to build entity from template with name '{templateName}': this template does not registered in system");
            }

            return result;
        }
    }
}
