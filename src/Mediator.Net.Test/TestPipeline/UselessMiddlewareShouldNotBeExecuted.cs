﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Net.Binding;
using Mediator.Net.Test.CommandHandlers;
using Mediator.Net.Test.Messages;
using Mediator.Net.Test.Middlewares;
using Mediator.Net.Test.TestUtils;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Mediator.Net.Test.TestPipeline
{
    [Collection("Avoid parallel execution")]
    public class UselessMiddlewareShouldNotBeExecuted : TestBase
    {
        private IMediator _mediator;
        public void GivenAMediator()
        {
            ClearBinding();
            var binding = new List<MessageBinding>() { new MessageBinding( typeof(TestBaseCommand), typeof(TestBaseCommandHandler) ) };
            var builder = new MediatorBuilder();
            _mediator = builder.RegisterHandlers(binding)
                .ConfigureCommandReceivePipe(x =>
            {
                x.UseConsoleLogger2();
                x.UseConsoleLogger1();
                x.UseUselessMiddleware();
            })
            .Build();
            

        }

        public async Task WhenACommandIsSent()
        {
            await _mediator.SendAsync(new TestBaseCommand(Guid.NewGuid()));
        }

        public void ThenItShouldSkipTheUselessMiddleware()
        {
            RubishBox.Rublish.Count.ShouldBe(3);
            RubishBox.Rublish[0].ShouldBe(nameof(ConsoleLog2.UseConsoleLogger2));
            RubishBox.Rublish[1].ShouldBe(nameof(ConsoleLog1.UseConsoleLogger1));
            RubishBox.Rublish[2].ShouldBe(nameof(TestBaseCommandHandler));
        }

        [Fact]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
